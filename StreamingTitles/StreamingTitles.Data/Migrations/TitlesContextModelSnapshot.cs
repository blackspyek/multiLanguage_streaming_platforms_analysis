﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StreamingTitles.Data.Model;

#nullable disable

namespace StreamingTitles.Data.Migrations
{
    [DbContext(typeof(TitlesContext))]
    partial class TitlesContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("StreamingTitles.Data.Model.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Category");
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

                    b.Property<string>("Cast")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("Date_Added")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Director")
                        .IsRequired()
                        .HasColumnType("longtext");

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

            modelBuilder.Entity("StreamingTitles.Data.Model.Platform", b =>
                {
                    b.Navigation("TitlePlatform");
                });

            modelBuilder.Entity("StreamingTitles.Data.Model.Title", b =>
                {
                    b.Navigation("TitleCategory");

                    b.Navigation("TitlePlatform");
                });
#pragma warning restore 612, 618
        }
    }
}
