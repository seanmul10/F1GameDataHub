CREATE TABLE IF NOT EXISTS session_metadata (
    session_uid BIGINT PRIMARY KEY,
    app_context_id UUID NOT NULL,
    player_car_index INT NOT NULL,
    game_year INT NOT NULL,
    packet_format INT NOT NULL,
    track_id INT NOT NULL,
    session_type TEXT NOT NULL,
    session_length TEXT NOT NULL,
    formula TEXT NOT NULL,
    session_start_time TIMESTAMPTZ NOT NULL
);
