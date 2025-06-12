CREATE VIEW vErasCalculationByPoll AS
WITH PercentageCalc AS (
    SELECT
        a.poll_variable_id,
        answer_text,
        ROUND(
            (COUNT(answer_text) * 100.0) /
            SUM(COUNT(answer_text)) OVER (PARTITION BY a.poll_variable_id),
            2
        ) AS answer_percentage,
        COUNT(answer_text) AS answer_count
    FROM
        answers a
    GROUP BY
        a.poll_variable_id, answer_text
),
RiskAverageByComponent AS (
    SELECT
        c."name" AS component_name,
        ROUND(AVG(a.risk_level),2) AS average_risk
    FROM
        answers a
    JOIN poll_variable pv ON a.poll_variable_id = pv."Id"
    JOIN variables v ON pv.variable_id = v."Id"
    JOIN components c ON v.component_id = c."Id"
    GROUP BY
        c."name" /*, pv.poll_id Repeating result by some reason*/
),
RiskAverageByVariable AS (
    SELECT
        v."Id" AS variable_id,
        v."name" AS variable_name,
        ROUND(AVG(a.risk_level),2) AS average_risk
    FROM
        answers a
    JOIN poll_variable pv ON a.poll_variable_id = pv."Id"
    JOIN variables v ON pv.variable_id = v."Id"
    GROUP BY
        v."Id", v."name" /*, pv.poll_id Repeating result by some reason*/
),
RiskCountByPollInstance AS (
    SELECT
        a.poll_instance_id,
        s."name" AS student_name,
        s."email" AS student_email,
        SUM(a.risk_level) AS poll_instance_risk_sum,
        COUNT(a.risk_level) AS poll_instance_answers_count
    FROM
        answers a
    JOIN poll_instances pi2 ON pi2."Id" = a.poll_instance_id
    JOIN students s ON s."Id" = pi2."StudentId"
    GROUP BY
        a.poll_instance_id, s."name", s."email"
),
RiskAvgByCohortComponent AS (
    SELECT
        sc.cohort_id,
        c."Id" AS component_id,
        ROUND(AVG(a.risk_level),2) AS average_risk_by_cohort_component
    FROM
        answers a
    JOIN poll_variable pv ON pv."Id" = a.poll_variable_id
    JOIN variables v ON v."Id" = pv.variable_id
    JOIN components c ON c."Id" = v.component_id
    JOIN poll_instances pi ON pi."Id" = a.poll_instance_id
    JOIN student_cohort sc ON sc.student_id = pi."StudentId"
    GROUP BY
        sc.cohort_id, c."Id"
)
select
	p."Id" AS poll_id,
    p."uuid" AS poll_uuid,
    v.component_id,
    c."name" AS component_name,
    pc.poll_variable_id,
    v."name" AS question,
    pc.answer_text,
    a.poll_instance_id,
    sc.student_id,
    rcbi.student_name,
    rcbi.student_email,
    a.risk_level AS answer_risk,
    rcbi.poll_instance_risk_sum,
    rcbi.poll_instance_answers_count,
    rac.average_risk AS component_average_risk,
    rav.average_risk AS variable_average_risk,
    pc.answer_count,
    pc.answer_percentage,
    sc.cohort_id,
    coh."name" AS cohort_name,
    racbc.average_risk_by_cohort_component,
    a.version_number as poll_version
FROM
    answers a
JOIN poll_variable pv ON a.poll_variable_id = pv."Id"
JOIN variables v ON pv.variable_id = v."Id"
JOIN components c ON v.component_id = c."Id"
JOIN polls p ON pv.poll_id = p."Id"
JOIN poll_instances pi ON a.poll_instance_id = pi."Id"
JOIN student_cohort sc ON sc.student_id = pi."StudentId"
JOIN cohorts coh ON coh."Id" = sc.cohort_id
JOIN PercentageCalc pc ON a.poll_variable_id = pc.poll_variable_id AND a.answer_text = pc.answer_text
JOIN RiskAverageByComponent rac ON c."name" = rac.component_name
JOIN RiskAverageByVariable rav ON v."Id" = rav.variable_id
JOIN RiskCountByPollInstance rcbi ON a.poll_instance_id = rcbi.poll_instance_id
JOIN RiskAvgByCohortComponent racbc ON racbc.cohort_id = sc.cohort_id AND racbc.component_id = c."Id"
GROUP by
	p."Id",
    poll_uuid,
    v.component_id,
    component_name,
    pc.poll_variable_id,
    question,
    pc.answer_text,
    a.poll_instance_id,
    sc.student_id,
    rcbi.student_name,
    rcbi.student_email,
    pc.answer_count,
    pc.answer_percentage,
    rcbi.poll_instance_risk_sum,
    rcbi.poll_instance_answers_count,
    rac.average_risk,
    rav.average_risk,
    sc.cohort_id,
    coh."name",
    racbc.average_risk_by_cohort_component,
    c."name",
    a.risk_level,
    poll_version
ORDER BY
    pc.poll_variable_id,
    pc.answer_percentage DESC;
