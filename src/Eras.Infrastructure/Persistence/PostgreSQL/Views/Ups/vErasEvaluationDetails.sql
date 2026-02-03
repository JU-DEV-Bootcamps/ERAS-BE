CREATE VIEW vErasEvaluationDetails AS
SELECT DISTINCT
    e."Id" AS EvaluationId,
    e.name AS EvaluationName,
    e.start_date AS StartDate,
    e.end_date AS EndDate,
    e.status AS Status,
    p."Id" AS PollId,
    p.name AS PollName,
    p.uuid AS PollUuid,
    pi."Id" AS PollInstanceId,
    pi."FinishedAt",
    s."Id" AS StudentId,
    s.name AS StudentName,
    s.email AS StudentEmail,
    sc.cohort_id AS CohortId,
    max(ans."Id") AS AnswerId,
    ans.answer_text AS AnswerText,
    ans.risk_level AS RiskLevel,
    v."Id" AS VariableId,
    v.name AS VariableName,
    comp."Id" AS ComponentId,
    comp.name AS ComponentName,
    pv.version_number AS VariableVersion
FROM evaluation e
JOIN evaluation_poll ep ON e."Id" = ep.evaluation_id
JOIN polls p ON ep.poll_id = p."Id"
LEFT JOIN poll_instances pi ON p.uuid = pi.uuid
LEFT JOIN students s ON pi."StudentId" = s."Id"
LEFT JOIN student_cohort sc ON s."Id" = sc.student_id
LEFT JOIN answers ans ON pi."Id" = ans.poll_instance_id
LEFT JOIN poll_variable pv ON ans.poll_variable_id = pv."Id"
LEFT JOIN variables v ON pv.variable_id = v."Id"
LEFT JOIN components comp ON v.component_id = comp."Id"
GROUP BY 
    e."Id", e.name, e.start_date, e.end_date, e.status,
    p."Id", p.name, p.uuid, pi."Id", pi."FinishedAt",
    s."Id", s.name, s.email, ans.answer_text, ans.risk_level,
    v."Id", v.name, comp."Id", comp.name, pv.version_number, CohortId;