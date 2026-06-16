CREATE TABLE IF NOT EXISTS replay_sessions (
    session_uid BIGINT NOT NULL,
    frame_id INTEGER NOT NULL,
    packet_format INTEGER NOT NULL,
    game_year INTEGER NOT NULL,
    packet_timestamp TIMESTAMPTZ NOT NULL,
    track_id INTEGER NULL,
    session_type TEXT NULL,
    session_length TEXT NULL,
    formula TEXT NULL,
    packet_json JSONB NOT NULL DEFAULT '{}'::jsonb,
    PRIMARY KEY (session_uid, frame_id)
);

CREATE TABLE IF NOT EXISTS replay_events (
    session_uid BIGINT NOT NULL,
    frame_id INTEGER NOT NULL,
    packet_format INTEGER NOT NULL,
    game_year INTEGER NOT NULL,
    packet_timestamp TIMESTAMPTZ NOT NULL,
    event_code TEXT NOT NULL,
    packet_json JSONB NOT NULL DEFAULT '{}'::jsonb,
    PRIMARY KEY (session_uid, frame_id, event_code)
);

CREATE TABLE IF NOT EXISTS replay_telemetry (
    session_uid BIGINT NOT NULL,
    frame_id INTEGER NOT NULL,
    driver_index INTEGER NOT NULL,
    packet_format INTEGER NOT NULL,
    game_year INTEGER NOT NULL,
    packet_timestamp TIMESTAMPTZ NOT NULL,
    throttle REAL NOT NULL,
    brake REAL NOT NULL,
    steering REAL NOT NULL,
    speed INTEGER NOT NULL,
    PRIMARY KEY (session_uid, frame_id, driver_index)
);

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM pg_proc WHERE proname = 'create_hypertable') THEN
        PERFORM create_hypertable('replay_sessions', 'frame_id', if_not_exists => TRUE);
        PERFORM create_hypertable('replay_events', 'frame_id', if_not_exists => TRUE);
        PERFORM create_hypertable('replay_telemetry', 'frame_id', if_not_exists => TRUE);
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS idx_replay_sessions_session_uid_frame
    ON replay_sessions (session_uid, frame_id);

CREATE INDEX IF NOT EXISTS idx_replay_events_session_uid_frame
    ON replay_events (session_uid, frame_id);

CREATE INDEX IF NOT EXISTS idx_replay_telemetry_session_uid_frame
    ON replay_telemetry (session_uid, frame_id);
