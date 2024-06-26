﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StreamingTitles.Data.Model;

#nullable disable

namespace StreamingTitles.Data.Migrations
{
    [DbContext(typeof(TitlesContext))]
    [Migration("20240614134853_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("StreamingTitles.Data.Model.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Category");
                });

            modelBuilder.Entity("StreamingTitles.Data.Model.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Country");
                });

            modelBuilder.Entity("StreamingTitles.Data.Model.Platform", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Platforms");
                });

            modelBuilder.Entity("StreamingTitles.Data.Model.Title", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("Release_Year")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("TitleName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Title");
                });

            modelBuilder.Entity("StreamingTitles.Data.Model.TitleCategory", b =>
                {
                    b.Property<int>("TitleId")
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.HasKey("TitleId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("TitleCategories");
                });

            modelBuilder.Entity("StreamingTitles.Data.Model.TitleCountry", b =>
                {
                    b.Property<int>("TitleId")
                        .HasColumnType("int");

                    b.Property<int>("CountryId")
                        .HasColumnType("int");

                    b.HasKey("TitleId", "CountryId");

                    b.HasIndex("CountryId");

                    b.ToTable("TitleCountry");
                });

            modelBuilder.Entity("StreamingTitles.Data.Model.TitlePlatform", b =>
                {
                    b.Property<int>("TitleId")
                        .HasColumnType("int");

                    b.Property<int>("PlatformId")
                        .HasColumnType("int");

                    b.HasKey("TitleId", "PlatformId");

                    b.HasIndex("PlatformId");

                    b.ToTable("TitlePlatform");
                });

            modelBuilder.Entity("StreamingTitles.Data.Model.TitleCategory", b =>
                {
                    b.HasOne("StreamingTitles.Data.Model.Category", "Category")
                        .WithMany("TitleCategory")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StreamingTitles.Data.Model.Title", "Title")
                        .WithMany("TitleCategory")
                        .HasForeignKey("TitleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Title");
                });

            modelBuilder.Entity("StreamingTitles.Data.Model.TitleCountry", b =>
                {
                    b.HasOne("StreamingTitles.Data.Model.Country", "Country")
                        .WithMany("TitleCountry")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StreamingTitles.Data.Model.Title", "Title")
                        .WithMany("TitleCountry")
                        .HasForeignKey("TitleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Country");

                    b.Navigation("Title");
                });

            modelBuilder.Entity("StreamingTitles.Data.Model.TitlePlatform", b =>
                {
                    b.HasOne("StreamingTitles.Data.Model.Platform", "Platform")
                        .WithMany("TitlePlatform")
                        .HasForeignKey("PlatformId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StreamingTitles.Data.Model.Title", "Title")
                        .WithMany("TitlePlatform")
                        .HasForeignKey("TitleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Platform");

                    b.Navigation("Title");
                });

            modelBuilder.Entity("StreamingTitles.Data.Model.Category", b =>
                {
                    b.Navigation("TitleCategory");
                });

            modelBuilder.Entity("StreamingTitles.Data.Model.Country", b =>
                {
                    b.Navigation("TitleCountry");
                });

            modelBuilder.Entity("StreamingTitles.Data.Model.Platform", b =>
                {
                    b.Navigation("TitlePlatform");
                });

            modelBuilder.Entity("StreamingTitles.Data.Model.Title", b =>
                {
                    b.Navigation("TitleCategory");

                    b.Navigation("TitleCountry");

                    b.Navigation("TitlePlatform");
                });
#pragma warning restore 612, 618
        }
    }
}
