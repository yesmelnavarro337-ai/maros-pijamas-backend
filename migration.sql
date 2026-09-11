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

