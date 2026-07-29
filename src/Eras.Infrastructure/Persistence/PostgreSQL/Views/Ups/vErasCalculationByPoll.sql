DROP VIEW IF EXISTS vErasCalculationByPoll;
CREATE VIEW vErasCalculationByPoll AS
WITH ResolvedAnswers AS (
    SELECT 
        a."Id", a.poll_instance_id, a.poll_variable_id, a.answer_text, a.risk_level, a.version_number
    FROM answers a
    JOIN poll_instances pi ON pi."Id" = a.poll_instance_id
    WHERE pi."SourcePollInstanceId" IS NULL
    UNION ALL
    SELECT
        a."Id", pi."Id" AS poll_instance_id, a.poll_variable_id, a.answer_text, a.risk_level, a.version_number
    FROM poll_instances pi
    JOIN answers a ON a.poll_instance_id = pi."SourcePollInstanceId"
    WHERE pi."SourcePollInstanceId" IS NOT NULL
),
PercentageCalc AS (
    SELECT
        a.poll_variable_id, answer_text,
        ROUND((COUNT(answer_text) * 100.0) / SUM(COUNT(answer_text)) OVER (PARTITION BY a.poll_variable_id), 2) AS answer_percentage,
        COUNT(answer_text) AS answer_count
    FROM ResolvedAnswers a
    WHERE a.answer_text NOT IN ('-', '', 'None', 'none', 'Ninguno', 'ninguno', 'Ninguna', 'ninguna') AND a.answer_text IS NOT NULL
    GROUP BY a.poll_variable_id, answer_text
),
RiskAverageByComponent AS (
    SELECT
        pv.poll_id, c."name" AS component_name,
        ROUND(AVG(a.risk_level),2) AS average_risk
    FROM ResolvedAnswers a
    JOIN poll_variable pv ON a.poll_variable_id = pv."Id"
    JOIN variables v ON pv.variable_id = v."Id"
    JOIN components c ON v.component_id = c."Id"
    WHERE a.answer_text NOT IN ('-', '', 'None', 'none', 'Ninguno', 'ninguno', 'Ninguna', 'ninguna') AND a.answer_text IS NOT NULL
    GROUP BY pv.poll_id, c."name"
),
RiskAverageByVariable AS (
    SELECT
        v."Id" AS variable_id,
        ROUND(AVG(a.risk_level),2) AS average_risk
    FROM ResolvedAnswers a
    JOIN poll_variable pv ON a.poll_variable_id = pv."Id"
    JOIN variables v ON pv.variable_id = v."Id"
    WHERE a.answer_text NOT IN ('-', '', 'None', 'none', 'Ninguno', 'ninguno', 'Ninguna', 'ninguna') AND a.answer_text IS NOT NULL
    GROUP BY v."Id"
),
RiskCountByPollInstance AS (
    SELECT
        a.poll_instance_id, s."name" AS student_name, s."email" AS student_email,
        SUM(a.risk_level) AS poll_instance_risk_sum,
        COUNT(a.risk_level) AS poll_instance_answers_count
    FROM ResolvedAnswers a
    JOIN poll_instances pi2 ON pi2."Id" = a.poll_instance_id
    JOIN students s ON s."Id" = pi2."StudentId"
    GROUP BY a.poll_instance_id, s."name", s."email"
),
RiskAvgByCohortComponent AS (
    SELECT
        sc.cohort_id, c."Id" AS component_id,
        ROUND(AVG(a.risk_level),2) AS average_risk_by_cohort_component
    FROM ResolvedAnswers a
    JOIN poll_variable pv ON pv."Id" = a.poll_variable_id
    JOIN variables v ON v."Id" = pv.variable_id
    JOIN components c ON c."Id" = v.component_id
    JOIN poll_instances pi ON pi."Id" = a.poll_instance_id
    JOIN student_cohort sc ON sc.student_id = pi."StudentId"
    GROUP BY sc.cohort_id, c."Id"
)
SELECT
    p."Id" AS poll_id,
    p."uuid" AS poll_uuid,
    v.component_id,
    c."name" AS component_name,
    a.poll_variable_id,
    v."name" AS question,
    v.position AS position,
    a.answer_text,
    a.poll_instance_id,
    sc.student_id,
    rcbi.student_name,
    rcbi.student_email,
    a.risk_level AS answer_risk,
    COALESCE(rcbi.poll_instance_risk_sum, 0) AS poll_instance_risk_sum,
    COALESCE(rcbi.poll_instance_answers_count, 0) AS poll_instance_answers_count,
    COALESCE(rac.average_risk, 0) AS component_average_risk,
    COALESCE(rav.average_risk, 0) AS variable_average_risk,
    COALESCE(pc.answer_count, 0) AS answer_count,
    COALESCE(pc.answer_percentage, 0) AS answer_percentage,
    sc.cohort_id,
    coh."name" AS cohort_name,
    COALESCE(racbc.average_risk_by_cohort_component, 0) AS average_risk_by_cohort_component,
    a.version_number AS poll_version
FROM ResolvedAnswers a
JOIN poll_variable pv ON a.poll_variable_id = pv."Id"
JOIN variables v ON pv.variable_id = v."Id"
JOIN components c ON v.component_id = c."Id"
JOIN polls p ON pv.poll_id = p."Id"
JOIN poll_instances pi ON a.poll_instance_id = pi."Id"
JOIN student_cohort sc ON sc.student_id = pi."StudentId"
JOIN cohorts coh ON coh."Id" = sc.cohort_id
LEFT JOIN PercentageCalc pc ON a.poll_variable_id = pc.poll_variable_id AND a.answer_text = pc.answer_text
LEFT JOIN RiskAverageByComponent rac ON c."name" = rac.component_name AND p."Id" = rac.poll_id
LEFT JOIN RiskAverageByVariable rav ON v."Id" = rav.variable_id
LEFT JOIN RiskCountByPollInstance rcbi ON a.poll_instance_id = rcbi.poll_instance_id
LEFT JOIN RiskAvgByCohortComponent racbc ON racbc.cohort_id = sc.cohort_id AND racbc.component_id = c."Id"
GROUP BY
    p."Id", p."uuid", v.component_id, c."name", a.poll_variable_id,
    v."name", v.position, a.answer_text, a.poll_instance_id, sc.student_id,
    rcbi.student_name, rcbi.student_email, a.risk_level, 
    rcbi.poll_instance_risk_sum, rcbi.poll_instance_answers_count,
    rac.average_risk, rav.average_risk, pc.answer_count, pc.answer_percentage,
    sc.cohort_id, coh."name", racbc.average_risk_by_cohort_component, a.version_number
ORDER BY a.poll_variable_id, pc.answer_percentage DESC;