START TRANSACTION;
ALTER TABLE "ProductCategories" ADD "Price" numeric(18,2);

ALTER TABLE "ProductCategories" ADD "SurchargeReason" character varying(500);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260928163314_AddCategoryPricing', '10.0.10');

COMMIT;

