START TRANSACTION;
ALTER TABLE "Products" ALTER COLUMN "SeoDescription" TYPE character varying(500);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260928205140_UpdateSeoDescriptionLength', '10.0.10');

COMMIT;

