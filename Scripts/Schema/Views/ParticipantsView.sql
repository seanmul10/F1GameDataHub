CREATE OR REPLACE VIEW ParticipantsView AS
SELECT
    p.session_uid,
    p.driver_id,
    p.network_id,
    p.name,
    t.name as team,
    n.name as nationality,
    p.is_ai_controlled,
    p.race_number
FROM Participants p
LEFT JOIN Teams t
       ON p.team_id = t.id
LEFT JOIN Nationalities n
       ON p.nationality_id = n.id;
