-- ============================================================================
-- Maro's Pijamas — Esquema + Datos (PostgreSQL)
-- Generado para Supabase / PostgreSQL a partir de la migración EF InitialPostgres
-- y de los datos actuales de la base de producción (exportación real).
-- ----------------------------------------------------------------------------
-- Uso en Supabase:
--   Dashboard > SQL Editor > New query > pegar todo > Run
-- O por CLI:
--   psql "postgresql://user:pass@host:5432/dbname" -f schema_and_data.sql
--
-- Orden: primero el esquema (START TRANSACTION ... COMMIT), luego los datos
-- en orden de dependencias de llaves foráneas (padres antes que hijos).
-- ============================================================================
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE "BlogPosts" (
    "Id" uuid NOT NULL,
    "Title" character varying(200) NOT NULL,
    "Slug" character varying(220) NOT NULL,
    "Category" character varying(60) NOT NULL,
    "CoverImageUrl" text,
    "Content" text NOT NULL,
    "Status" character varying(20) NOT NULL,
    "PublishDate" timestamp with time zone NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_BlogPosts" PRIMARY KEY ("Id")
);

CREATE TABLE "Categories" (
    "Id" uuid NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Slug" character varying(120) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Categories" PRIMARY KEY ("Id")
);

CREATE TABLE "Collections" (
    "Id" uuid NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Description" text NOT NULL,
    "CoverImageUrl" text,
    "AccentHex" character varying(7) NOT NULL,
    "IsDefault" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Collections" PRIMARY KEY ("Id")
);

CREATE TABLE "ContactMessages" (
    "Id" uuid NOT NULL,
    "FullName" character varying(150) NOT NULL,
    "Phone" character varying(30) NOT NULL,
    "Email" character varying(256),
    "Message" character varying(1000) NOT NULL,
    "IsRead" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_ContactMessages" PRIMARY KEY ("Id")
);

CREATE TABLE "Customers" (
    "Id" uuid NOT NULL,
    "Name" character varying(150) NOT NULL,
    "Phone" character varying(30) NOT NULL,
    "Email" character varying(256),
    "City" character varying(100) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Customers" PRIMARY KEY ("Id")
);

CREATE TABLE "CustomizationOptions" (
    "Id" uuid NOT NULL,
    "CatalogType" character varying(20) NOT NULL,
    "Name" character varying(100) NOT NULL,
    "ImageUrl" text,
    "ColorHex" character varying(7),
    "PriceModifier" numeric(10,2),
    "Active" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_CustomizationOptions" PRIMARY KEY ("Id")
);

CREATE TABLE "Faqs" (
    "Id" uuid NOT NULL,
    "Question" character varying(300) NOT NULL,
    "Answer" character varying(1000) NOT NULL,
    "Category" character varying(60) NOT NULL,
    "Order" integer NOT NULL,
    "Status" character varying(20) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Faqs" PRIMARY KEY ("Id")
);

CREATE TABLE "GalleryImages" (
    "Id" uuid NOT NULL,
    "Url" text NOT NULL,
    "PublicId" character varying(300) NOT NULL,
    "Category" character varying(20) NOT NULL,
    "Caption" character varying(300) NOT NULL,
    "Order" integer NOT NULL,
    "Active" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_GalleryImages" PRIMARY KEY ("Id")
);

CREATE TABLE "PageHeaders" (
    "Id" uuid NOT NULL,
    "PageKey" character varying(60) NOT NULL,
    "Title" character varying(200) NOT NULL,
    "Subtitle" character varying(500) NOT NULL,
    "BackgroundImageUrl" character varying(500),
    "PrimaryButtonText" character varying(80),
    "PrimaryButtonLink" character varying(300),
    "SecondaryButtonText" character varying(80),
    "SecondaryButtonLink" character varying(300),
    "TextColor" character varying(20) NOT NULL,
    "OverlayOpacity" integer NOT NULL DEFAULT 40,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_PageHeaders" PRIMARY KEY ("Id")
);

CREATE TABLE "SiteSettings" (
    "Id" uuid NOT NULL,
    "SiteName" character varying(150) NOT NULL,
    "Description" text NOT NULL,
    "Currency" text NOT NULL,
    "Timezone" text NOT NULL,
    "Language" text NOT NULL,
    "MaintenanceMode" boolean NOT NULL,
    "LogoUrl" text,
    "Instagram" text,
    "Facebook" text,
    "TikTok" text,
    "WhatsappNumber" character varying(30) NOT NULL,
    "WhatsappDefaultMessage" text NOT NULL,
    "Address" character varying(300) NOT NULL,
    "BusinessHours" character varying(200) NOT NULL,
    "EmailFromName" text NOT NULL,
    "EmailFromAddress" character varying(256) NOT NULL,
    "NotifyNewQuotation" boolean NOT NULL,
    "SeoMetaTitle" text NOT NULL,
    "SeoMetaDescription" text NOT NULL,
    "SeoSocialImageUrl" text,
    "LegalTermsUrl" text NOT NULL,
    "LegalPrivacyUrl" text NOT NULL,
    "LegalReturnsPolicy" text NOT NULL,
    "CustomDomain" character varying(150) NOT NULL,
    "SslEnabled" boolean NOT NULL,
    "AutoBackupEnabled" boolean NOT NULL,
    "BackupFrequency" text NOT NULL,
    "LastBackupDate" timestamp with time zone,
    "TwoFactorEnabled" boolean NOT NULL,
    "SessionTimeoutMinutes" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_SiteSettings" PRIMARY KEY ("Id")
);

CREATE TABLE "Testimonials" (
    "Id" uuid NOT NULL,
    "ClientName" character varying(150) NOT NULL,
    "Rating" integer NOT NULL,
    "Quote" character varying(500) NOT NULL,
    "Status" character varying(20) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Testimonials" PRIMARY KEY ("Id")
);

CREATE TABLE "Users" (
    "Id" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Email" character varying(256) NOT NULL,
    "PasswordHash" text NOT NULL,
    "Role" character varying(30) NOT NULL,
    "Status" character varying(20) NOT NULL,
    "LastAccessAt" timestamp with time zone,
    "InviteToken" character varying(64),
    "InviteTokenExpiresAt" timestamp with time zone,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

CREATE TABLE "Products" (
    "Id" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Slug" character varying(220) NOT NULL,
    "CategoryId" uuid NOT NULL,
    "Description" text NOT NULL,
    "BasePrice" numeric(12,2) NOT NULL,
    "Status" character varying(20) NOT NULL,
    "FeaturedHome" boolean NOT NULL,
    "AllowCustomization" boolean NOT NULL,
    "DeliveryTime" character varying(100) NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "SeoTitle" character varying(200) NOT NULL,
    "SeoDescription" character varying(300) NOT NULL,
    "SeoSlug" character varying(220) NOT NULL,
    "SeoSocialImageUrl" text,
    "SeoAltText" text,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Products" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Products_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Seasons" (
    "Id" uuid NOT NULL,
    "Name" character varying(150) NOT NULL,
    "Slug" character varying(170) NOT NULL,
    "StartDate" timestamp with time zone NOT NULL,
    "EndDate" timestamp with time zone NOT NULL,
    "Status" character varying(20) NOT NULL,
    "CollectionId" uuid NOT NULL,
    "HeroTitle" character varying(200) NOT NULL,
    "HeroSubtitle" character varying(300) NOT NULL,
    "HeroImageUrl" text,
    "BannerImageUrl" text,
    "ColorPrimary" character varying(7) NOT NULL,
    "ColorAccent" character varying(7) NOT NULL,
    "ColorBackground" character varying(7) NOT NULL,
    "CtaText" character varying(60) NOT NULL,
    "CtaLink" character varying(200) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Seasons" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Seasons_Collections_CollectionId" FOREIGN KEY ("CollectionId") REFERENCES "Collections" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Quotations" (
    "Id" uuid NOT NULL,
    "CustomerId" uuid NOT NULL,
    "Status" character varying(20) NOT NULL,
    "Notes" character varying(1000) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Quotations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Quotations_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "ProductCollections" (
    "ProductId" uuid NOT NULL,
    "CollectionId" uuid NOT NULL,
    CONSTRAINT "PK_ProductCollections" PRIMARY KEY ("ProductId", "CollectionId"),
    CONSTRAINT "FK_ProductCollections_Collections_CollectionId" FOREIGN KEY ("CollectionId") REFERENCES "Collections" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProductCollections_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ProductImages" (
    "Id" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    "Url" text NOT NULL,
    "Order" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_ProductImages" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ProductImages_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ProductVariants" (
    "Id" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    "Size" character varying(20) NOT NULL,
    "ColorName" character varying(60) NOT NULL,
    "ColorHex" character varying(7) NOT NULL,
    "Sku" character varying(60) NOT NULL,
    "Stock" integer NOT NULL,
    "ImageUrl" text,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_ProductVariants" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ProductVariants_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Banners" (
    "Id" uuid NOT NULL,
    "Title" character varying(200) NOT NULL,
    "ImageUrl" text,
    "LinkUrl" character varying(300) NOT NULL,
    "Position" character varying(60) NOT NULL,
    "Active" boolean NOT NULL,
    "StartDate" timestamp with time zone,
    "EndDate" timestamp with time zone,
    "SeasonId" uuid,
    "CollectionId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Banners" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Banners_Collections_CollectionId" FOREIGN KEY ("CollectionId") REFERENCES "Collections" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Banners_Seasons_SeasonId" FOREIGN KEY ("SeasonId") REFERENCES "Seasons" ("Id") ON DELETE SET NULL
);

CREATE TABLE "SeasonFeaturedProducts" (
    "SeasonId" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    CONSTRAINT "PK_SeasonFeaturedProducts" PRIMARY KEY ("SeasonId", "ProductId"),
    CONSTRAINT "FK_SeasonFeaturedProducts_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_SeasonFeaturedProducts_Seasons_SeasonId" FOREIGN KEY ("SeasonId") REFERENCES "Seasons" ("Id") ON DELETE CASCADE
);

CREATE TABLE "QuotationItems" (
    "Id" uuid NOT NULL,
    "QuotationId" uuid NOT NULL,
    "ProductId" uuid,
    "Size" character varying(20) NOT NULL,
    "Quantity" integer NOT NULL,
    "EmbroideryText" text,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_QuotationItems" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_QuotationItems_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_QuotationItems_Quotations_QuotationId" FOREIGN KEY ("QuotationId") REFERENCES "Quotations" ("Id") ON DELETE CASCADE
);

CREATE TABLE "QuotationReferenceImages" (
    "Id" uuid NOT NULL,
    "QuotationId" uuid NOT NULL,
    "Url" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_QuotationReferenceImages" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_QuotationReferenceImages_Quotations_QuotationId" FOREIGN KEY ("QuotationId") REFERENCES "Quotations" ("Id") ON DELETE CASCADE
);

CREATE TABLE "QuotationItemOptions" (
    "QuotationItemId" uuid NOT NULL,
    "CustomizationOptionId" uuid NOT NULL,
    CONSTRAINT "PK_QuotationItemOptions" PRIMARY KEY ("QuotationItemId", "CustomizationOptionId"),
    CONSTRAINT "FK_QuotationItemOptions_CustomizationOptions_CustomizationOpti~" FOREIGN KEY ("CustomizationOptionId") REFERENCES "CustomizationOptions" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_QuotationItemOptions_QuotationItems_QuotationItemId" FOREIGN KEY ("QuotationItemId") REFERENCES "QuotationItems" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Banners_CollectionId" ON "Banners" ("CollectionId");

CREATE INDEX "IX_Banners_SeasonId" ON "Banners" ("SeasonId");

CREATE UNIQUE INDEX "IX_BlogPosts_Slug" ON "BlogPosts" ("Slug");

CREATE UNIQUE INDEX "IX_Categories_Slug" ON "Categories" ("Slug");

CREATE UNIQUE INDEX "IX_PageHeaders_PageKey" ON "PageHeaders" ("PageKey");

CREATE INDEX "IX_ProductCollections_CollectionId" ON "ProductCollections" ("CollectionId");

CREATE INDEX "IX_ProductImages_ProductId" ON "ProductImages" ("ProductId");

CREATE INDEX "IX_Products_CategoryId" ON "Products" ("CategoryId");

CREATE INDEX "IX_Products_IsDeleted" ON "Products" ("IsDeleted");

CREATE UNIQUE INDEX "IX_Products_Slug" ON "Products" ("Slug");

CREATE INDEX "IX_ProductVariants_ProductId" ON "ProductVariants" ("ProductId");

CREATE UNIQUE INDEX "IX_ProductVariants_Sku" ON "ProductVariants" ("Sku");

CREATE INDEX "IX_QuotationItemOptions_CustomizationOptionId" ON "QuotationItemOptions" ("CustomizationOptionId");

CREATE INDEX "IX_QuotationItems_ProductId" ON "QuotationItems" ("ProductId");

CREATE INDEX "IX_QuotationItems_QuotationId" ON "QuotationItems" ("QuotationId");

CREATE INDEX "IX_QuotationReferenceImages_QuotationId" ON "QuotationReferenceImages" ("QuotationId");

CREATE INDEX "IX_Quotations_CustomerId" ON "Quotations" ("CustomerId");

CREATE INDEX "IX_SeasonFeaturedProducts_ProductId" ON "SeasonFeaturedProducts" ("ProductId");

CREATE INDEX "IX_Seasons_CollectionId" ON "Seasons" ("CollectionId");

CREATE UNIQUE INDEX "IX_Seasons_Slug" ON "Seasons" ("Slug");

CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");

CREATE UNIQUE INDEX "IX_Users_InviteToken" ON "Users" ("InviteToken");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260911065350_InitialPostgres', '10.0.10');

COMMIT;



-- ============================================================================
-- SECCIÓN DE DATOS (SEEDING / DATA)
-- ============================================================================

INSERT INTO "Users" ("Id", "Name", "Email", "PasswordHash", "Role", "Status", "LastAccessAt", "CreatedAt", "UpdatedAt", "InviteToken", "InviteTokenExpiresAt") VALUES ('7cd833ee-77df-4f17-bfa4-3bf68b5655e7', 'Camila Pérez', 'camila@marospijamas.com', 'AQAAAAIAAYagAAAAELCvY6OPhOSt1ep8IHpb7uh5BvjfRLmeEndcjE+MyVYd2Rru2z4pEtU9BZMjw9BfCw==', 'Editor', 'Pendiente', NULL, '2026-08-09 07:44:07+00:00', NULL, NULL, NULL);
INSERT INTO "Users" ("Id", "Name", "Email", "PasswordHash", "Role", "Status", "LastAccessAt", "CreatedAt", "UpdatedAt", "InviteToken", "InviteTokenExpiresAt") VALUES ('5a6faba5-16f8-40fe-8d95-7ed569a12e5b', 'Yoniris Navarro', 'yoniris0509@gmail.com', 'AQAAAAIAAYagAAAAEFtwSPznT5DfvCvkxezV9SDK2ZuG5WPfTGstR6LO+1IP+PJfO8FWRiGT5XKuu22r1g==', 'Viewer', 'Pendiente', NULL, '2026-09-09 17:44:24+00:00', NULL, NULL, NULL);
INSERT INTO "Users" ("Id", "Name", "Email", "PasswordHash", "Role", "Status", "LastAccessAt", "CreatedAt", "UpdatedAt", "InviteToken", "InviteTokenExpiresAt") VALUES ('b92a9b0a-06bd-4b2e-a7c9-8289ea6b7e81', 'Administradora (editado)', 'yesmelnavarro337@gmail.com', 'AQAAAAIAAYagAAAAEKMRaWiRzjAUzV/u//z+642Casms6u7bP3xY34ub6eUJZcOpqvepxJwQrt/yfokMAA==', 'Administrador', 'Activo', '2026-09-11 05:51:39+00:00', '2026-08-09 07:07:22+00:00', '2026-09-11 00:19:20+00:00', NULL, NULL);
INSERT INTO "Users" ("Id", "Name", "Email", "PasswordHash", "Role", "Status", "LastAccessAt", "CreatedAt", "UpdatedAt", "InviteToken", "InviteTokenExpiresAt") VALUES ('f28e7fce-7882-4cf0-9345-fa55a90cd042', 'Yesmel', 'yesmeljose337@gmail.com', 'AQAAAAIAAYagAAAAEDvixrgcfa9lEN1VlOKPArVbENzLcwrF61fOgxUthbfvyQ69eGBKPL14IZHy5f63cw==', 'Editor', 'Pendiente', NULL, '2026-09-09 17:46:24+00:00', NULL, NULL, NULL);
INSERT INTO "Users" ("Id", "Name", "Email", "PasswordHash", "Role", "Status", "LastAccessAt", "CreatedAt", "UpdatedAt", "InviteToken", "InviteTokenExpiresAt") VALUES ('3d91ce41-beb5-421d-ac0c-fc3e5c7b9795', 'Prueba', 'vroomlyinventary@gmail.com', 'AQAAAAIAAYagAAAAEPJTov6Ci5qhnoUQEM5V/ckxzII3gsBln2AiBfy/fPonkztsRh7Z+abBA6e4MiYQ+g==', 'Viewer', 'Pendiente', NULL, '2026-09-11 00:16:58+00:00', NULL, 'oiROwV2_KFXrO4oXVSK-rqNcz_y1hZgG0yss5LsBRio', '2026-09-13 00:16:58+00:00');
INSERT INTO "SiteSettings" ("Id", "SiteName", "Description", "Currency", "Timezone", "Language", "MaintenanceMode", "LogoUrl", "Instagram", "Facebook", "TikTok", "WhatsappNumber", "WhatsappDefaultMessage", "EmailFromName", "EmailFromAddress", "NotifyNewQuotation", "SeoMetaTitle", "SeoMetaDescription", "SeoSocialImageUrl", "LegalTermsUrl", "LegalPrivacyUrl", "LegalReturnsPolicy", "CustomDomain", "SslEnabled", "AutoBackupEnabled", "BackupFrequency", "LastBackupDate", "TwoFactorEnabled", "SessionTimeoutMinutes", "CreatedAt", "UpdatedAt", "Address", "BusinessHours") VALUES ('d9d4f01b-8e3b-4d98-960f-261dafb98005', 'Maro''s Pijamas', 'Tienda oficial de pijamas y ropa de descanso', 'COP - Peso Colombiano', 'UTC-05:00 Bogotá', 'Español', FALSE, NULL, 'https://www.instagram.com/maros_m.a?stkn=MTIwNGF6NmFwZ3piYw==', NULL, NULL, '+573013169974', 'Hola, quisiera solicitar información sobre sus pijamas.', 'Maro''s Pijamas', 'contacto@marospijamas.com', TRUE, 'Maro''s Pijamas | Comodidad y Estilo', 'Las mejores pijamas para toda la familia con envíos a todo el país.', NULL, '/terminos-y-condiciones', '/politica-de-privacidad', '/politica-de-devoluciones', 'marospijamas.com', TRUE, TRUE, 'semanal', NULL, TRUE, 60, '2026-08-10 02:57:05+00:00', '2026-09-11 05:09:12+00:00', '', '');
INSERT INTO "Categories" ("Id", "Name", "Slug", "CreatedAt", "UpdatedAt") VALUES ('2d2f49a1-0924-436e-bc39-026a20eb3962', 'Batas', 'batas', '2026-09-10 22:12:04+00:00', NULL);
INSERT INTO "Categories" ("Id", "Name", "Slug", "CreatedAt", "UpdatedAt") VALUES ('ae642477-4f63-4a7c-b2af-1940e8e2c80c', 'Parejas', 'parejas', '2026-09-10 22:11:25+00:00', NULL);
INSERT INTO "Categories" ("Id", "Name", "Slug", "CreatedAt", "UpdatedAt") VALUES ('5efae840-812e-4449-b8de-5a267d97f008', 'ASDFS', 'asdfs', '2026-08-09 07:38:50+00:00', '2026-09-10 22:09:16+00:00');
INSERT INTO "Categories" ("Id", "Name", "Slug", "CreatedAt", "UpdatedAt") VALUES ('9f3fbf05-318b-409d-bff7-5f3b348b4522', 'Familia', 'familia', '2026-09-10 22:11:40+00:00', NULL);
INSERT INTO "Categories" ("Id", "Name", "Slug", "CreatedAt", "UpdatedAt") VALUES ('56d9668e-baf5-48c1-b206-8ad56f74961b', 'Mujer', 'mujer', '2026-09-10 22:10:57+00:00', NULL);
INSERT INTO "Categories" ("Id", "Name", "Slug", "CreatedAt", "UpdatedAt") VALUES ('c0b2d630-484f-402d-b9aa-ab040e3195a4', 'Hombre', 'hombre', '2026-09-10 22:11:16+00:00', NULL);
INSERT INTO "Categories" ("Id", "Name", "Slug", "CreatedAt", "UpdatedAt") VALUES ('5ad1b8f2-ac65-4013-a757-c9fcc1f68a82', 'Niños y Bebes', 'ninos-y-bebes', '2026-09-10 22:11:51+00:00', NULL);
INSERT INTO "Collections" ("Id", "Name", "Description", "CoverImageUrl", "AccentHex", "IsDefault", "CreatedAt", "UpdatedAt") VALUES ('37b3ce29-e870-4fa1-96d6-81a69bdaad7b', 'Navidad ', 'Coleccion navideña 2026', NULL, '#6B6832', FALSE, '2026-08-09 16:51:43+00:00', '2026-08-09 17:11:10+00:00');
INSERT INTO "Collections" ("Id", "Name", "Description", "CoverImageUrl", "AccentHex", "IsDefault", "CreatedAt", "UpdatedAt") VALUES ('9434ec89-10ea-4f9d-aae3-a617441fb991', 'General', 'Catálogo permanente', NULL, '#6B6832', FALSE, '2026-08-09 16:22:22+00:00', '2026-08-13 01:17:22+00:00');
INSERT INTO "Collections" ("Id", "Name", "Description", "CoverImageUrl", "AccentHex", "IsDefault", "CreatedAt", "UpdatedAt") VALUES ('cab2357b-0349-4ebc-96a9-cdc2558630af', 'Amor y Amistad 2026', 'Celebra los lazos más especiales con nuestra nueva colección de pijamas diseñadas para compartir. Descubre prendas suaves, frescas y con detalles llenos de cariño, ideales para regalar o regalarte. Porque los mejores momentos se viven en casa, envuelto en la máxima comodidad y con el estilo que mereces. ¡Haz que cada noche sea una excusa para celebrar el afecto!!', 'https://res.cloudinary.com/am8qkv09/image/upload/v1788828803/maros-pijamas/collections/phcfucqxj0umfugq0v0i.jpg', '#ff0000', TRUE, '2026-09-08 00:52:15+00:00', '2026-09-11 04:59:39+00:00');
INSERT INTO "Products" ("Id", "Name", "Slug", "CategoryId", "Description", "BasePrice", "Status", "FeaturedHome", "AllowCustomization", "DeliveryTime", "SeoTitle", "SeoDescription", "SeoSlug", "SeoSocialImageUrl", "SeoAltText", "CreatedAt", "UpdatedAt", "IsDeleted") VALUES ('59b077d9-2aab-4a76-b0e4-7627d4617f44', 'Pijama Clásica Satín Manga Larga', 'pijama-clasica-satin-manga-larga', '5efae840-812e-4449-b8de-5a267d97f008', 'Pijama de satín suave y elegante, incluye camisa de botones con cuello camisero y pantalón con pretina elástica. Máximo confort para el descanso.', 129000.00, 'Activo', TRUE, TRUE, '5-7 días hábiles', 'Pijama Clásica de Satín para Mujer | Maro''s Pijamas', 'Descubre la comodidad y elegancia con nuestra pijama de satín de manga larga. Envíos a todo el país.', 'pijama-clasica-satin-manga-larga', NULL, NULL, '2026-08-09 17:03:34+00:00', NULL, FALSE);
INSERT INTO "Products" ("Id", "Name", "Slug", "CategoryId", "Description", "BasePrice", "Status", "FeaturedHome", "AllowCustomization", "DeliveryTime", "SeoTitle", "SeoDescription", "SeoSlug", "SeoSocialImageUrl", "SeoAltText", "CreatedAt", "UpdatedAt", "IsDeleted") VALUES ('328af121-f320-4c98-b49b-bda3fba50554', 'Pijama Clásica Satin Navideña Esmeralda', 'pijama-clasica-satin-navidena-esmeralda', '5efae840-812e-4449-b8de-5a267d97f008', 'Edición especial navideña. Conjunto de pijama de dos piezas confeccionado en satén premium de tono verde esmeralda con ribetes a contraste en rojo festivo. Camisa de manga larga con cuello de solapa, abotonada al frente y bolsillo en el pecho, acompañada de un pantalón de corte recto de ajuste cómodo. Perfecta para lucir elegante y acogedora durante las fiestas de fin de año.', 135000.00, 'Activo', TRUE, TRUE, '5-7 días hábiles', '', '', 'pijama-clasica-satin-navidena-esmeralda', NULL, NULL, '2026-09-03 04:56:57+00:00', NULL, FALSE);
INSERT INTO "Products" ("Id", "Name", "Slug", "CategoryId", "Description", "BasePrice", "Status", "FeaturedHome", "AllowCustomization", "DeliveryTime", "SeoTitle", "SeoDescription", "SeoSlug", "SeoSocialImageUrl", "SeoAltText", "CreatedAt", "UpdatedAt", "IsDeleted") VALUES ('5307cdba-e5cd-429e-94af-d932f69116df', 'Pijama Navidad', 'pijama-navidad', '5efae840-812e-4449-b8de-5a267d97f008', 'Pijamas familiares para navidad', 60000.00, 'Activo', FALSE, TRUE, '5-7 días hábiles', '', '', 'pijama-navidad', NULL, NULL, '2026-08-12 19:50:00+00:00', NULL, FALSE);
INSERT INTO "Products" ("Id", "Name", "Slug", "CategoryId", "Description", "BasePrice", "Status", "FeaturedHome", "AllowCustomization", "DeliveryTime", "SeoTitle", "SeoDescription", "SeoSlug", "SeoSocialImageUrl", "SeoAltText", "CreatedAt", "UpdatedAt", "IsDeleted") VALUES ('c505094a-8df1-4124-9e30-dd93fe5366fa', 'Pijama Satín Beige', 'pijama-satin-beige', '5efae840-812e-4449-b8de-5a267d97f008', 'Pijama de dos piezas en satín suave.', 129000.00, 'Activo', TRUE, TRUE, '5-7 días hábiles', 'Pijama Satín Beige | Maro''s Pijamas', 'Pijama premium en satín.', 'pijama-satin-beige', NULL, NULL, '2026-08-09 07:41:58+00:00', NULL, FALSE);
INSERT INTO "ProductCollections" ("ProductId", "CollectionId") VALUES ('328af121-f320-4c98-b49b-bda3fba50554', '37b3ce29-e870-4fa1-96d6-81a69bdaad7b');
INSERT INTO "ProductCollections" ("ProductId", "CollectionId") VALUES ('5307cdba-e5cd-429e-94af-d932f69116df', '37b3ce29-e870-4fa1-96d6-81a69bdaad7b');
INSERT INTO "ProductCollections" ("ProductId", "CollectionId") VALUES ('c505094a-8df1-4124-9e30-dd93fe5366fa', '37b3ce29-e870-4fa1-96d6-81a69bdaad7b');
INSERT INTO "ProductCollections" ("ProductId", "CollectionId") VALUES ('c505094a-8df1-4124-9e30-dd93fe5366fa', '9434ec89-10ea-4f9d-aae3-a617441fb991');
INSERT INTO "ProductImages" ("Id", "ProductId", "Url", "Order", "CreatedAt", "UpdatedAt") VALUES ('ea13a7dc-5420-42a2-8d5f-1991b45fd802', '328af121-f320-4c98-b49b-bda3fba50554', 'https://res.cloudinary.com/am8qkv09/image/upload/v1788411324/maros-pijamas/products/wnvlxlx2ukpetvmnuprk.jpg', 0, '2026-09-03 04:56:57+00:00', NULL);
INSERT INTO "ProductImages" ("Id", "ProductId", "Url", "Order", "CreatedAt", "UpdatedAt") VALUES ('0866a5bc-0d0e-409b-95f6-1eef2b8f1edb', '5307cdba-e5cd-429e-94af-d932f69116df', 'https://res.cloudinary.com/am8qkv09/image/upload/v1786564106/maros-pijamas/products/iobkky9cojhvo8wktqfh.png', 0, '2026-08-12 19:50:00+00:00', NULL);
INSERT INTO "ProductImages" ("Id", "ProductId", "Url", "Order", "CreatedAt", "UpdatedAt") VALUES ('10b2abcf-4b38-4d80-9f4e-42c0e5367b6d', '328af121-f320-4c98-b49b-bda3fba50554', 'https://res.cloudinary.com/am8qkv09/image/upload/v1788411331/maros-pijamas/products/qpveznx8nrbnj52cdavb.jpg', 1, '2026-09-03 04:56:57+00:00', NULL);
INSERT INTO "ProductImages" ("Id", "ProductId", "Url", "Order", "CreatedAt", "UpdatedAt") VALUES ('49028cad-49db-489e-a492-e3e2a0e8a144', '328af121-f320-4c98-b49b-bda3fba50554', 'https://res.cloudinary.com/am8qkv09/image/upload/v1788411335/maros-pijamas/products/zw1htqmssep09i2vpqbt.jpg', 2, '2026-09-03 04:56:57+00:00', NULL);
INSERT INTO "ProductImages" ("Id", "ProductId", "Url", "Order", "CreatedAt", "UpdatedAt") VALUES ('3bca4fbb-cc24-4518-a79c-fa08159e9ba1', '328af121-f320-4c98-b49b-bda3fba50554', 'https://res.cloudinary.com/am8qkv09/image/upload/v1788411339/maros-pijamas/products/qobaie2w9zdpwu2531xs.jpg', 3, '2026-09-03 04:56:57+00:00', NULL);
INSERT INTO "ProductVariants" ("Id", "ProductId", "Size", "ColorName", "ColorHex", "Sku", "Stock", "ImageUrl", "CreatedAt", "UpdatedAt") VALUES ('f43efa4f-7fe8-4a24-b269-152c11456fba', '5307cdba-e5cd-429e-94af-d932f69116df', 'S', 'Blanco', '#ffffff', 'SKU-S-BLA', 0, NULL, '2026-08-12 19:50:00+00:00', NULL);
INSERT INTO "ProductVariants" ("Id", "ProductId", "Size", "ColorName", "ColorHex", "Sku", "Stock", "ImageUrl", "CreatedAt", "UpdatedAt") VALUES ('3c6bd701-b22f-4e4b-8c2e-1b5395c38545', '328af121-f320-4c98-b49b-bda3fba50554', 'L', 'Esmeralda', '#366c33', 'SKU-L-ESM', 5, NULL, '2026-09-03 04:56:57+00:00', NULL);
INSERT INTO "ProductVariants" ("Id", "ProductId", "Size", "ColorName", "ColorHex", "Sku", "Stock", "ImageUrl", "CreatedAt", "UpdatedAt") VALUES ('3801fe06-63a9-4758-a38e-22e299a95053', '59b077d9-2aab-4a76-b0e4-7627d4617f44', 'M', 'Beige', '#EFE8D8', 'PSB-XM-BEI', 8, NULL, '2026-08-09 17:03:34+00:00', NULL);
INSERT INTO "ProductVariants" ("Id", "ProductId", "Size", "ColorName", "ColorHex", "Sku", "Stock", "ImageUrl", "CreatedAt", "UpdatedAt") VALUES ('b0084b9f-c205-4bbc-8712-27802037af0b', '5307cdba-e5cd-429e-94af-d932f69116df', 'M', 'Blanco', '#ffffff', 'SKU-M-BLA', 0, NULL, '2026-08-12 19:50:00+00:00', NULL);
INSERT INTO "ProductVariants" ("Id", "ProductId", "Size", "ColorName", "ColorHex", "Sku", "Stock", "ImageUrl", "CreatedAt", "UpdatedAt") VALUES ('4dd7b2d1-a70c-40d1-bc02-4c87f6512970', 'c505094a-8df1-4124-9e30-dd93fe5366fa', 'S', 'Beige', '#EFE8D8', 'PSB-S-BEI', 10, NULL, '2026-08-09 07:41:58+00:00', NULL);
INSERT INTO "ProductVariants" ("Id", "ProductId", "Size", "ColorName", "ColorHex", "Sku", "Stock", "ImageUrl", "CreatedAt", "UpdatedAt") VALUES ('767e04c6-14af-46ee-abfd-9b487231ef55', 'c505094a-8df1-4124-9e30-dd93fe5366fa', 'M', 'Beige', '#EFE8D8', 'PSB-M-BEI', 8, NULL, '2026-08-09 07:41:58+00:00', NULL);
INSERT INTO "ProductVariants" ("Id", "ProductId", "Size", "ColorName", "ColorHex", "Sku", "Stock", "ImageUrl", "CreatedAt", "UpdatedAt") VALUES ('9eac3cf2-1157-47d6-adb4-b55af2951cd1', '5307cdba-e5cd-429e-94af-d932f69116df', 'S', 'Verde', '#29a337', 'SKU-S-VER', 0, NULL, '2026-08-12 19:50:00+00:00', NULL);
INSERT INTO "ProductVariants" ("Id", "ProductId", "Size", "ColorName", "ColorHex", "Sku", "Stock", "ImageUrl", "CreatedAt", "UpdatedAt") VALUES ('f158b946-dc02-4ae8-880a-c13a35b60623', '59b077d9-2aab-4a76-b0e4-7627d4617f44', 'S', 'Beige', '#EFE8D8', 'PSB-XS-BEI', 10, NULL, '2026-08-09 17:03:34+00:00', NULL);
INSERT INTO "ProductVariants" ("Id", "ProductId", "Size", "ColorName", "ColorHex", "Sku", "Stock", "ImageUrl", "CreatedAt", "UpdatedAt") VALUES ('89572292-364e-47cf-bb22-c209129ac00f', '328af121-f320-4c98-b49b-bda3fba50554', 'XL', 'Esmeralda', '#366c33', 'SKU-XL-ESM', 5, NULL, '2026-09-03 04:56:57+00:00', NULL);
INSERT INTO "ProductVariants" ("Id", "ProductId", "Size", "ColorName", "ColorHex", "Sku", "Stock", "ImageUrl", "CreatedAt", "UpdatedAt") VALUES ('1f1b9ab4-a9d6-40ea-a74d-cf105d330111', '328af121-f320-4c98-b49b-bda3fba50554', 'XS', 'Esmeralda', '#366c33', 'SKU-XS-ESM', 2, NULL, '2026-09-03 04:56:57+00:00', NULL);
INSERT INTO "ProductVariants" ("Id", "ProductId", "Size", "ColorName", "ColorHex", "Sku", "Stock", "ImageUrl", "CreatedAt", "UpdatedAt") VALUES ('990dc5fd-37cf-4e5c-abe5-e7989bec35a3', '328af121-f320-4c98-b49b-bda3fba50554', 'S', 'Esmeralda', '#366c33', 'SKU-S-ESM', 5, NULL, '2026-09-03 04:56:57+00:00', NULL);
INSERT INTO "ProductVariants" ("Id", "ProductId", "Size", "ColorName", "ColorHex", "Sku", "Stock", "ImageUrl", "CreatedAt", "UpdatedAt") VALUES ('a3f8d4c3-721d-4ed7-a677-e93869d46960', '5307cdba-e5cd-429e-94af-d932f69116df', 'M', 'Verde', '#29a337', 'SKU-M-VER', 0, NULL, '2026-08-12 19:50:00+00:00', NULL);
INSERT INTO "ProductVariants" ("Id", "ProductId", "Size", "ColorName", "ColorHex", "Sku", "Stock", "ImageUrl", "CreatedAt", "UpdatedAt") VALUES ('fb07f54b-1783-4a4a-b195-fab236ae3500', '328af121-f320-4c98-b49b-bda3fba50554', 'M', 'Esmeralda', '#366c33', 'SKU-M-ESM', 5, NULL, '2026-09-03 04:56:57+00:00', NULL);
INSERT INTO "Seasons" ("Id", "Name", "Slug", "StartDate", "EndDate", "Status", "CollectionId", "HeroTitle", "HeroSubtitle", "HeroImageUrl", "BannerImageUrl", "ColorPrimary", "ColorAccent", "ColorBackground", "CtaText", "CtaLink", "CreatedAt", "UpdatedAt") VALUES ('0b51c904-7e76-48ea-802b-2e4de5251baf', 'Amor y Amistad 2026', 'amor-y-amistad-2026', '2026-09-01', '2026-09-30', 'Finalizada', 'cab2357b-0349-4ebc-96a9-cdc2558630af', 'Celebra la Magia de Estar Juntos', 'Descubre nuestra colección de pijamas para Amor y Amistad, diseñadas para compartir momentos inolvidables con quien más quieres.', 'https://res.cloudinary.com/am8qkv09/image/upload/v1788828979/maros-pijamas/general/ky8cqkqg0m9izhjwzfrz.jpg', 'https://res.cloudinary.com/am8qkv09/image/upload/v1788828998/maros-pijamas/general/qajrfk6oiweyzcggx065.jpg', '#ff0033', '#d8a7b1', '#f5ebe6', 'Ver colección', '/coleccion/amor-amistad', '2026-09-08 00:59:05+00:00', '2026-09-08 00:59:53+00:00');
INSERT INTO "Seasons" ("Id", "Name", "Slug", "StartDate", "EndDate", "Status", "CollectionId", "HeroTitle", "HeroSubtitle", "HeroImageUrl", "BannerImageUrl", "ColorPrimary", "ColorAccent", "ColorBackground", "CtaText", "CtaLink", "CreatedAt", "UpdatedAt") VALUES ('9880877c-96b9-49d7-b75c-7f86b63b55be', 'Halloween 2026', 'halloween-2026', '2026-09-20', '2026-10-31', 'Activa', '9434ec89-10ea-4f9d-aae3-a617441fb991', 'Asusta en este halloween', 'Descubre la colección de halloween', 'https://res.cloudinary.com/am8qkv09/image/upload/v1789101579/maros-pijamas/seasons/quqypm20bfz5d17vrvvi.png', 'https://res.cloudinary.com/am8qkv09/image/upload/v1786594593/maros-pijamas/seasons/ui3pdevpigtumai3ieqv.png', '#6B6832', '#ff9500', '#FAF8F4', 'Ver colección', '/coleccion/halloween', '2026-08-10 02:38:34+00:00', '2026-09-11 04:39:43+00:00');
INSERT INTO "Seasons" ("Id", "Name", "Slug", "StartDate", "EndDate", "Status", "CollectionId", "HeroTitle", "HeroSubtitle", "HeroImageUrl", "BannerImageUrl", "ColorPrimary", "ColorAccent", "ColorBackground", "CtaText", "CtaLink", "CreatedAt", "UpdatedAt") VALUES ('767d7b0c-9c59-4b2e-9f45-a2499639af96', 'Navidad 2026', 'navidad-2026', '2026-11-15', '2026-12-31', 'Finalizada', '37b3ce29-e870-4fa1-96d6-81a69bdaad7b', 'Duerme como un sueño esta Navidad', 'Descubre la colección navideña', NULL, NULL, '#6B6832', '#B6AE3A', '#FAF8F4', 'Ver colección', '/coleccion/navidad', '2026-08-09 17:12:34+00:00', '2026-08-31 19:36:42+00:00');
INSERT INTO "SeasonFeaturedProducts" ("SeasonId", "ProductId") VALUES ('9880877c-96b9-49d7-b75c-7f86b63b55be', 'c505094a-8df1-4124-9e30-dd93fe5366fa');
INSERT INTO "SeasonFeaturedProducts" ("SeasonId", "ProductId") VALUES ('767d7b0c-9c59-4b2e-9f45-a2499639af96', 'c505094a-8df1-4124-9e30-dd93fe5366fa');
INSERT INTO "Banners" ("Id", "Title", "ImageUrl", "LinkUrl", "Position", "Active", "CreatedAt", "UpdatedAt", "CollectionId", "EndDate", "SeasonId", "StartDate") VALUES ('8758e50a-81e7-42ce-a6cf-1990ee29ce0d', 'Bordados gratuitos para Halloween', 'https://res.cloudinary.com/am8qkv09/image/upload/v1788976375/maros-pijamas/banners/idtlqtxygdhxvxoffrcw.jpg', '/coleccion/halloween', 'Home - Medio', FALSE, '2026-09-09 17:54:02+00:00', '2026-09-11 04:51:24+00:00', NULL, '2026-10-31', '9880877c-96b9-49d7-b75c-7f86b63b55be', '2026-09-09');
INSERT INTO "Banners" ("Id", "Title", "ImageUrl", "LinkUrl", "Position", "Active", "CreatedAt", "UpdatedAt", "CollectionId", "EndDate", "SeasonId", "StartDate") VALUES ('c58657c9-96ca-4317-8f04-c245e7770da3', 'Envío gratis +$150.000', NULL, '/coleccion/general', 'Home - Superior', TRUE, '2026-08-10 02:13:59+00:00', NULL, NULL, NULL, NULL, NULL);
INSERT INTO "Banners" ("Id", "Title", "ImageUrl", "LinkUrl", "Position", "Active", "CreatedAt", "UpdatedAt", "CollectionId", "EndDate", "SeasonId", "StartDate") VALUES ('d26477df-5938-4152-afde-e8818e0a7e23', 'Nueva colección Navidad', NULL, '/coleccion/navidad', 'Home - Medio', TRUE, '2026-08-10 02:23:13+00:00', NULL, NULL, NULL, '767d7b0c-9c59-4b2e-9f45-a2499639af96', NULL);
INSERT INTO "Customers" ("Id", "Name", "Phone", "Email", "City", "CreatedAt", "UpdatedAt") VALUES ('7331ea5e-7dc2-4870-855d-2880a7b6ea76', 'Yesmel Jose De La Torre Navarro', '3024630390', 'yesmelnavarro337@gmail.com', 'Valledupar', '2026-09-03 04:33:07+00:00', NULL);
INSERT INTO "Customers" ("Id", "Name", "Phone", "Email", "City", "CreatedAt", "UpdatedAt") VALUES ('9e8b2b79-b2ab-45ec-81a0-a42cdbea3861', 'María López', '3001234567', 'maria@gmail.com', 'Valledupar', '2026-08-09 17:58:36+00:00', NULL);
INSERT INTO "Customers" ("Id", "Name", "Phone", "Email", "City", "CreatedAt", "UpdatedAt") VALUES ('3bc857d1-d415-41d3-a092-d2205bf638b0', 'Yanecxy De La Torre', '3242862923', 'yesmelnavarro337@gmail.com', 'Valledupar', '2026-09-08 19:28:39+00:00', NULL);
INSERT INTO "CustomizationOptions" ("Id", "CatalogType", "Name", "ImageUrl", "ColorHex", "PriceModifier", "Active", "CreatedAt", "UpdatedAt") VALUES ('7c9fc3c6-2e81-46b7-bd46-10d2b66e0b06', 'Color', 'Color', NULL, '#6B6832', NULL, FALSE, '2026-08-09 17:35:25+00:00', '2026-08-09 17:38:54+00:00');
INSERT INTO "CustomizationOptions" ("Id", "CatalogType", "Name", "ImageUrl", "ColorHex", "PriceModifier", "Active", "CreatedAt", "UpdatedAt") VALUES ('d71468dc-186d-40c0-94ac-14e84f51fffe', 'Tela', 'Satín', NULL, NULL, 20000.00, TRUE, '2026-08-09 17:32:23+00:00', NULL);
INSERT INTO "CustomizationOptions" ("Id", "CatalogType", "Name", "ImageUrl", "ColorHex", "PriceModifier", "Active", "CreatedAt", "UpdatedAt") VALUES ('e389908f-8b53-46ca-b362-2166af1b82f5', 'Estampado', 'Floral', NULL, NULL, 5000.00, TRUE, '2026-08-12 19:23:42+00:00', NULL);
INSERT INTO "CustomizationOptions" ("Id", "CatalogType", "Name", "ImageUrl", "ColorHex", "PriceModifier", "Active", "CreatedAt", "UpdatedAt") VALUES ('d82f3a98-488e-4f54-8b77-2f3340954e1e', 'Modelo', 'Escaneados', NULL, NULL, NULL, TRUE, '2026-08-12 19:22:57+00:00', '2026-08-12 19:24:22+00:00');
INSERT INTO "CustomizationOptions" ("Id", "CatalogType", "Name", "ImageUrl", "ColorHex", "PriceModifier", "Active", "CreatedAt", "UpdatedAt") VALUES ('65531a1f-cd80-4ea4-894e-bb31f4d0a05d', 'Talla', 'XS', NULL, NULL, NULL, TRUE, '2026-08-12 19:24:01+00:00', NULL);
INSERT INTO "CustomizationOptions" ("Id", "CatalogType", "Name", "ImageUrl", "ColorHex", "PriceModifier", "Active", "CreatedAt", "UpdatedAt") VALUES ('931f34e1-aef1-4aff-849e-dfba69ec64c2', 'Bordado', 'Personalizados', NULL, NULL, 10000.00, TRUE, '2026-08-12 19:23:56+00:00', NULL);
INSERT INTO "CustomizationOptions" ("Id", "CatalogType", "Name", "ImageUrl", "ColorHex", "PriceModifier", "Active", "CreatedAt", "UpdatedAt") VALUES ('d44ed6c3-f5c3-4b14-b217-f563c50eebc3', 'Color', 'Azul', NULL, '#0010f5', NULL, TRUE, '2026-08-12 19:23:22+00:00', NULL);
INSERT INTO "Quotations" ("Id", "CustomerId", "Status", "Notes", "CreatedAt", "UpdatedAt") VALUES ('a5f7388f-9be2-484c-a68b-06029eb9d7cd', '7331ea5e-7dc2-4870-855d-2880a7b6ea76', 'Nueva', 'Nada', '2026-09-10 17:46:12+00:00', NULL);
INSERT INTO "Quotations" ("Id", "CustomerId", "Status", "Notes", "CreatedAt", "UpdatedAt") VALUES ('91cc4c37-bb61-4b29-a2d5-155fc6e7ab3e', '7331ea5e-7dc2-4870-855d-2880a7b6ea76', 'Nueva', 'Ningún detalle adicional.', '2026-09-03 04:33:07+00:00', NULL);
INSERT INTO "Quotations" ("Id", "CustomerId", "Status", "Notes", "CreatedAt", "UpdatedAt") VALUES ('c09c95b6-3884-4f43-b7bc-25834cf6401c', '7331ea5e-7dc2-4870-855d-2880a7b6ea76', 'Nueva', '', '2026-09-09 19:02:20+00:00', NULL);
INSERT INTO "Quotations" ("Id", "CustomerId", "Status", "Notes", "CreatedAt", "UpdatedAt") VALUES ('42d07e27-2a0d-4642-b2d8-33fa5f7f4b73', '3bc857d1-d415-41d3-a092-d2205bf638b0', 'Contactada', 'Nada adicional.', '2026-09-08 19:28:39+00:00', '2026-09-08 19:32:51+00:00');
INSERT INTO "Quotations" ("Id", "CustomerId", "Status", "Notes", "CreatedAt", "UpdatedAt") VALUES ('b97d3102-dedd-4050-b1d5-4027c91283fc', '9e8b2b79-b2ab-45ec-81a0-a42cdbea3861', 'Nueva', 'Preguntó por envío a Valledupar.', '2026-08-09 17:59:27+00:00', NULL);
INSERT INTO "Quotations" ("Id", "CustomerId", "Status", "Notes", "CreatedAt", "UpdatedAt") VALUES ('f944584a-68f2-4755-84b9-b4717a7621be', '9e8b2b79-b2ab-45ec-81a0-a42cdbea3861', 'Nueva', 'Preguntó por envío a Valledupar.', '2026-08-09 17:58:36+00:00', NULL);
INSERT INTO "Quotations" ("Id", "CustomerId", "Status", "Notes", "CreatedAt", "UpdatedAt") VALUES ('5f9d899a-9145-4845-b2b1-baeb6bc532ae', '9e8b2b79-b2ab-45ec-81a0-a42cdbea3861', 'Contactada', 'Preguntó por envío a Valledupar.', '2026-08-09 17:59:31+00:00', '2026-08-09 18:03:43+00:00');
INSERT INTO "Quotations" ("Id", "CustomerId", "Status", "Notes", "CreatedAt", "UpdatedAt") VALUES ('b3e1fa8b-353c-41d6-8a5a-d46a344cd3bf', '7331ea5e-7dc2-4870-855d-2880a7b6ea76', 'Contactada', 'Nada.', '2026-09-03 05:05:31+00:00', '2026-09-08 19:33:19+00:00');
INSERT INTO "Quotations" ("Id", "CustomerId", "Status", "Notes", "CreatedAt", "UpdatedAt") VALUES ('915962d1-0c5d-4bfd-9b56-e3ed93fbe763', '7331ea5e-7dc2-4870-855d-2880a7b6ea76', 'Nueva', 'Nada', '2026-09-10 17:41:57+00:00', NULL);
INSERT INTO "QuotationItems" ("Id", "QuotationId", "ProductId", "Size", "Quantity", "CreatedAt", "UpdatedAt", "EmbroideryText") VALUES ('4ca484ca-e805-4c69-8542-1dc05e56a7d4', '42d07e27-2a0d-4642-b2d8-33fa5f7f4b73', '328af121-f320-4c98-b49b-bda3fba50554', 'S', 1, '2026-09-08 19:28:39+00:00', NULL, 'Y.D');
INSERT INTO "QuotationItems" ("Id", "QuotationId", "ProductId", "Size", "Quantity", "CreatedAt", "UpdatedAt", "EmbroideryText") VALUES ('ce905c01-6bb1-404f-bb72-3124a7d0157a', 'c09c95b6-3884-4f43-b7bc-25834cf6401c', '328af121-f320-4c98-b49b-bda3fba50554', 'L', 1, '2026-09-09 19:02:20+00:00', NULL, 'Y.D');
INSERT INTO "QuotationItems" ("Id", "QuotationId", "ProductId", "Size", "Quantity", "CreatedAt", "UpdatedAt", "EmbroideryText") VALUES ('4aaa2f26-4ac1-4856-8d14-325ff67810c7', 'a5f7388f-9be2-484c-a68b-06029eb9d7cd', '5307cdba-e5cd-429e-94af-d932f69116df', 'S', 1, '2026-09-10 17:46:12+00:00', NULL, NULL);
INSERT INTO "QuotationItems" ("Id", "QuotationId", "ProductId", "Size", "Quantity", "CreatedAt", "UpdatedAt", "EmbroideryText") VALUES ('7d4efdc3-cdfe-429a-8d3b-3a045fd6965b', 'a5f7388f-9be2-484c-a68b-06029eb9d7cd', '328af121-f320-4c98-b49b-bda3fba50554', 'L', 1, '2026-09-10 17:46:12+00:00', NULL, NULL);
INSERT INTO "QuotationItems" ("Id", "QuotationId", "ProductId", "Size", "Quantity", "CreatedAt", "UpdatedAt", "EmbroideryText") VALUES ('b08592d1-6ab8-4e2b-bf62-b20ccf547358', '5f9d899a-9145-4845-b2b1-baeb6bc532ae', '59b077d9-2aab-4a76-b0e4-7627d4617f44', 'M', 1, '2026-08-09 17:59:31+00:00', NULL, NULL);
INSERT INTO "QuotationItems" ("Id", "QuotationId", "ProductId", "Size", "Quantity", "CreatedAt", "UpdatedAt", "EmbroideryText") VALUES ('7067951f-0324-48a9-9618-bc8b9771970f', 'b97d3102-dedd-4050-b1d5-4027c91283fc', '59b077d9-2aab-4a76-b0e4-7627d4617f44', 'M', 1, '2026-08-09 17:59:27+00:00', NULL, NULL);
INSERT INTO "QuotationItems" ("Id", "QuotationId", "ProductId", "Size", "Quantity", "CreatedAt", "UpdatedAt", "EmbroideryText") VALUES ('7525fc30-3210-4b02-9170-c1f2cc14e143', 'f944584a-68f2-4755-84b9-b4717a7621be', 'c505094a-8df1-4124-9e30-dd93fe5366fa', 'M', 1, '2026-08-09 17:58:36+00:00', NULL, NULL);
INSERT INTO "QuotationItems" ("Id", "QuotationId", "ProductId", "Size", "Quantity", "CreatedAt", "UpdatedAt", "EmbroideryText") VALUES ('2f8c4948-505e-44e4-8398-d61567092d38', 'b3e1fa8b-353c-41d6-8a5a-d46a344cd3bf', '328af121-f320-4c98-b49b-bda3fba50554', 'L', 1, '2026-09-03 05:05:31+00:00', NULL, 'YD');
INSERT INTO "QuotationItems" ("Id", "QuotationId", "ProductId", "Size", "Quantity", "CreatedAt", "UpdatedAt", "EmbroideryText") VALUES ('e6987592-932c-4d69-8392-e43b1b2a304d', '915962d1-0c5d-4bfd-9b56-e3ed93fbe763', '328af121-f320-4c98-b49b-bda3fba50554', 'L', 2, '2026-09-10 17:41:57+00:00', NULL, NULL);
INSERT INTO "QuotationItems" ("Id", "QuotationId", "ProductId", "Size", "Quantity", "CreatedAt", "UpdatedAt", "EmbroideryText") VALUES ('846514a9-4b3d-4536-8862-eb9447e0a5aa', '91cc4c37-bb61-4b29-a2d5-155fc6e7ab3e', '5307cdba-e5cd-429e-94af-d932f69116df', 'S', 1, '2026-09-03 04:33:07+00:00', NULL, 'Yesmel Navarro');
INSERT INTO "QuotationItemOptions" ("QuotationItemId", "CustomizationOptionId") VALUES ('4ca484ca-e805-4c69-8542-1dc05e56a7d4', 'd71468dc-186d-40c0-94ac-14e84f51fffe');
INSERT INTO "QuotationItemOptions" ("QuotationItemId", "CustomizationOptionId") VALUES ('ce905c01-6bb1-404f-bb72-3124a7d0157a', 'd71468dc-186d-40c0-94ac-14e84f51fffe');
INSERT INTO "QuotationItemOptions" ("QuotationItemId", "CustomizationOptionId") VALUES ('b08592d1-6ab8-4e2b-bf62-b20ccf547358', 'd71468dc-186d-40c0-94ac-14e84f51fffe');
INSERT INTO "QuotationItemOptions" ("QuotationItemId", "CustomizationOptionId") VALUES ('7067951f-0324-48a9-9618-bc8b9771970f', 'd71468dc-186d-40c0-94ac-14e84f51fffe');
INSERT INTO "QuotationItemOptions" ("QuotationItemId", "CustomizationOptionId") VALUES ('7525fc30-3210-4b02-9170-c1f2cc14e143', 'd71468dc-186d-40c0-94ac-14e84f51fffe');
INSERT INTO "QuotationItemOptions" ("QuotationItemId", "CustomizationOptionId") VALUES ('2f8c4948-505e-44e4-8398-d61567092d38', 'd71468dc-186d-40c0-94ac-14e84f51fffe');
INSERT INTO "QuotationItemOptions" ("QuotationItemId", "CustomizationOptionId") VALUES ('846514a9-4b3d-4536-8862-eb9447e0a5aa', 'd71468dc-186d-40c0-94ac-14e84f51fffe');
INSERT INTO "QuotationItemOptions" ("QuotationItemId", "CustomizationOptionId") VALUES ('4ca484ca-e805-4c69-8542-1dc05e56a7d4', 'e389908f-8b53-46ca-b362-2166af1b82f5');
INSERT INTO "QuotationItemOptions" ("QuotationItemId", "CustomizationOptionId") VALUES ('ce905c01-6bb1-404f-bb72-3124a7d0157a', 'e389908f-8b53-46ca-b362-2166af1b82f5');
INSERT INTO "QuotationItemOptions" ("QuotationItemId", "CustomizationOptionId") VALUES ('2f8c4948-505e-44e4-8398-d61567092d38', 'e389908f-8b53-46ca-b362-2166af1b82f5');
INSERT INTO "QuotationItemOptions" ("QuotationItemId", "CustomizationOptionId") VALUES ('846514a9-4b3d-4536-8862-eb9447e0a5aa', 'e389908f-8b53-46ca-b362-2166af1b82f5');
INSERT INTO "QuotationItemOptions" ("QuotationItemId", "CustomizationOptionId") VALUES ('4ca484ca-e805-4c69-8542-1dc05e56a7d4', '931f34e1-aef1-4aff-849e-dfba69ec64c2');
INSERT INTO "QuotationItemOptions" ("QuotationItemId", "CustomizationOptionId") VALUES ('ce905c01-6bb1-404f-bb72-3124a7d0157a', '931f34e1-aef1-4aff-849e-dfba69ec64c2');
INSERT INTO "QuotationItemOptions" ("QuotationItemId", "CustomizationOptionId") VALUES ('2f8c4948-505e-44e4-8398-d61567092d38', '931f34e1-aef1-4aff-849e-dfba69ec64c2');
INSERT INTO "QuotationItemOptions" ("QuotationItemId", "CustomizationOptionId") VALUES ('846514a9-4b3d-4536-8862-eb9447e0a5aa', '931f34e1-aef1-4aff-849e-dfba69ec64c2');
INSERT INTO "QuotationItemOptions" ("QuotationItemId", "CustomizationOptionId") VALUES ('4ca484ca-e805-4c69-8542-1dc05e56a7d4', 'd44ed6c3-f5c3-4b14-b217-f563c50eebc3');
INSERT INTO "QuotationItemOptions" ("QuotationItemId", "CustomizationOptionId") VALUES ('ce905c01-6bb1-404f-bb72-3124a7d0157a', 'd44ed6c3-f5c3-4b14-b217-f563c50eebc3');
INSERT INTO "QuotationItemOptions" ("QuotationItemId", "CustomizationOptionId") VALUES ('2f8c4948-505e-44e4-8398-d61567092d38', 'd44ed6c3-f5c3-4b14-b217-f563c50eebc3');
INSERT INTO "QuotationItemOptions" ("QuotationItemId", "CustomizationOptionId") VALUES ('846514a9-4b3d-4536-8862-eb9447e0a5aa', 'd44ed6c3-f5c3-4b14-b217-f563c50eebc3');
INSERT INTO "ContactMessages" ("Id", "FullName", "Phone", "Email", "Message", "IsRead", "CreatedAt", "UpdatedAt") VALUES ('9abb177f-9385-4ba9-b3e5-5c68028e27ee', 'Yesmel De La Torre', '302 463 03 90', 'yesmelnavarro337@gmail.com', 'Nada por el momento', FALSE, '2026-09-09 17:59:17+00:00', '2026-09-09 18:00:05+00:00');
INSERT INTO "Faqs" ("Id", "Question", "Answer", "Category", "Order", "Status", "CreatedAt", "UpdatedAt") VALUES ('122b8fb6-bf93-4d2c-9ef7-4dd2a7798f92', '¿Cómo puedo elegir la talla correcta de pijama?', 'Puedes revisar nuestra guía de tallas detallada en la descripción de cada producto.', 'Productos', 2, 'Publicada', '2026-08-09 22:58:31+00:00', NULL);
INSERT INTO "Faqs" ("Id", "Question", "Answer", "Category", "Order", "Status", "CreatedAt", "UpdatedAt") VALUES ('6ef9c113-6ffd-4caa-9b81-66722b722da0', '¿Cuáles son los métodos de pago disponibles?', 'Aceptamos tarjetas de crédito, débito, transferencias y Nequi.', 'Pagos', 1, 'Publicada', '2026-08-09 22:58:05+00:00', NULL);
INSERT INTO "Faqs" ("Id", "Question", "Answer", "Category", "Order", "Status", "CreatedAt", "UpdatedAt") VALUES ('fca86738-e9db-4833-9a31-efb3d7d4e609', '¿Realizan envíos a todo el país?', 'Sí, hacemos envíos a nivel nacional con entrega en 2 a 5 días hábiles.', 'Envíos', 0, 'Publicada', '2026-08-09 22:58:24+00:00', NULL);
INSERT INTO "PageHeaders" ("Id", "PageKey", "Title", "Subtitle", "BackgroundImageUrl", "PrimaryButtonText", "PrimaryButtonLink", "SecondaryButtonText", "SecondaryButtonLink", "TextColor", "OverlayOpacity", "CreatedAt", "UpdatedAt") VALUES ('22799a8b-0c1b-44a4-ad77-26dfe51a0843', 'collections', 'Nuestras Colecciones', 'Ediciones especiales disenadas con amor y confort para cada temporada.', NULL, NULL, NULL, NULL, NULL, '#F9F6F0', 40, '2026-09-11 03:20:29+00:00', '2026-09-11 03:25:45+00:00');
INSERT INTO "PageHeaders" ("Id", "PageKey", "Title", "Subtitle", "BackgroundImageUrl", "PrimaryButtonText", "PrimaryButtonLink", "SecondaryButtonText", "SecondaryButtonLink", "TextColor", "OverlayOpacity", "CreatedAt", "UpdatedAt") VALUES ('f9d28909-9bf6-4e74-b1c8-4ba8a6cb7233', 'blog', 'Blog', 'Consejos, inspiración y todo sobre pijamas personalizadas.', NULL, NULL, NULL, NULL, NULL, '#F9F6F0', 40, '2026-09-11 03:20:29+00:00', NULL);
INSERT INTO "PageHeaders" ("Id", "PageKey", "Title", "Subtitle", "BackgroundImageUrl", "PrimaryButtonText", "PrimaryButtonLink", "SecondaryButtonText", "SecondaryButtonLink", "TextColor", "OverlayOpacity", "CreatedAt", "UpdatedAt") VALUES ('4b1076c7-af19-4e01-bab9-627002ba67ca', 'gallery', 'Galería', 'Momentos especiales con Maro''s Pijamas.', NULL, NULL, NULL, NULL, NULL, '#F9F6F0', 40, '2026-09-11 03:20:29+00:00', NULL);
INSERT INTO "BlogPosts" ("Id", "Title", "Slug", "Category", "CoverImageUrl", "Content", "Status", "PublishDate", "CreatedAt", "UpdatedAt") VALUES ('404c0e2b-0137-4e4a-b7e7-d7cff57202ed', 'Colección Verano 2027', 'coleccion-verano-2027', 'Moda y Tendencias', 'https://res.cloudinary.com/am8qkv09/image/upload/v1786315507/maros-pijamas/blog/saft00xctzszzpj4fmgu.jpg', 'Próximamente...', 'Publicado', '2028-12-31', '2026-08-09 22:52:54+00:00', '2026-09-09 17:30:16+00:00');
INSERT INTO "Testimonials" ("Id", "ClientName", "Rating", "Quote", "Status", "CreatedAt", "UpdatedAt") VALUES ('f577e284-6c0c-41b5-8c67-bc2af1f7eed8', 'Yesmel De La Torre', 5, 'Excelente atención y la calidad de las pijamas es impecable. ¡Muy recomendado!', 'Publicado', '2026-08-09 22:55:14+00:00', '2026-08-09 22:56:45+00:00');
