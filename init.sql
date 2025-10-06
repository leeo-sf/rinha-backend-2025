CREATE UNLOGGED
TABLE payments (
    "correlationId" UUID PRIMARY KEY,
    "amount" NUMERIC NOT NULL,
    "requestedAt" TIMESTAMPTZ,
    "isProcessed" BOOLEAN NOT NULL,
    "processedBy" INTEGER
);