CREATE UNLOGGED
TABLE payments (
    correlationId UUID PRIMARY KEY,
    amount DECIMAL NOT NULL,
    requestedAt TIMESTAMP NOT NULL,
    isProcessed BOOLEAN NOT NULL,
    processedBy INTEGER NOT NULL
);