CREATE TABLE IF NOT EXISTS participants (
    session_uid      BIGINT   NOT NULL,
    driver_id        INT      NOT NULL,
    network_id       INT      NOT NULL,
    name             TEXT     NOT NULL,
    team_id          INT      NOT NULL,
    nationality_id   INT      NOT NULL,
    is_ai_controlled BOOLEAN  NOT NULL,
    race_number      INT      NOT NULL,

    PRIMARY KEY (session_uid, driver_id),
    FOREIGN KEY (session_uid) REFERENCES session_metadata(session_uid) ON DELETE CASCADE
);