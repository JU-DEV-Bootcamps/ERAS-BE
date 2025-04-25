CREATE OR REPLACE VIEW vErasCalculationByPoll AS
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
        AVG(a.risk_level) AS average_risk
    FROM
        answers a
    JOIN poll_variable pv ON a.poll_variable_id = pv."Id"
    JOIN variables v ON pv.variable_id = v."Id"
    JOIN components c ON v.component_id = c."Id"
    GROUP BY
        c."name"
),
RiskAverageByVariable AS (
    SELECT
        v."Id" AS variable_id,
        v."name" AS variable_name,
        AVG(a.risk_level) AS average_risk
    FROM
        answers a
    JOIN poll_variable pv ON a.poll_variable_id = pv."Id"
    JOIN variables v ON pv.variable_id = v."Id"
    GROUP BY
        v."Id", v."name"
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
    join poll_instances pi2 on pi2."Id"  = a.poll_instance_id
    join students s on s."Id" = pi2."StudentId"
    GROUP BY
        a.poll_instance_id, s."name", s."email"
)
SELECT
    p."uuid" AS poll_uuid,
    c."name" AS component_name,
    pc.poll_variable_id,
    v."name" AS question,
    pc.answer_text,
    a.poll_instance_id,
    rcbi.student_name,
    rcbi.student_email,
    a.risk_level AS answer_risk,
    rcbi.poll_instance_risk_sum,
    rcbi.poll_instance_answers_count,
    rac.average_risk AS component_average_risk,
    rav.average_risk AS variable_average_risk,
    pc.answer_count,
    pc.answer_percentage
FROM
    answers a
JOIN poll_variable pv ON a.poll_variable_id = pv."Id"
JOIN variables v ON pv.variable_id = v."Id"
JOIN components c ON v.component_id = c."Id"
JOIN polls p ON pv.poll_id = p."Id"
JOIN PercentageCalc pc ON a.poll_variable_id = pc.poll_variable_id AND a.answer_text = pc.answer_text
JOIN RiskAverageByComponent rac ON c."name" = rac.component_name
JOIN RiskAverageByVariable rav ON v."Id" = rav.variable_id
JOIN RiskCountByPollInstance rcbi ON a.poll_instance_id = rcbi.poll_instance_id
GROUP BY
    poll_uuid, component_name, pc.poll_variable_id, question, pc.answer_text, a.poll_instance_id, pc.answer_count, pc.answer_percentage, rcbi.poll_instance_risk_sum, rcbi.poll_instance_answers_count , rac.average_risk, rav.average_risk, c."name", a.risk_level, rcbi.student_name, rcbi.student_email
ORDER BY
    pc.poll_variable_id, pc.answer_percentage DESC;
