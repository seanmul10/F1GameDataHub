CREATE TABLE IF NOT EXISTS car_telemetry (
    session_uid     BIGINT   NOT NULL,
    frame_id        INTEGER  NOT NULL,
    driver_index    INT NOT NULL,
    
    throttle        REAL     NOT NULL,
    brake           REAL     NOT NULL,
    steering        REAL     NOT NULL,

    speed           INT NOT NULL,
    
    PRIMARY KEY (session_uid, frame_id, driver_index)
);
SELECT create_hypertable('car_telemetry', 'frame_id');