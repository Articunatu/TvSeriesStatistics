﻿// <auto-generated />
using System;
using AnimeStats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AnimeStats.Migrations
{
    [DbContext(typeof(DatabaseEFCore))]
    partial class DatabaseEFCoreModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AnimeStats.Service.Anime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Animes");
                });

            modelBuilder.Entity("AnimeStats.Service.Season", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<int>("AmtOfEpisodes")
                        .HasColumnType("int");

                    b.Property<int>("AmtOfMembers")
                        .HasColumnType("int");

                    b.Property<int>("AnimeId")
                        .HasColumnType("int");

                    b.Property<double>("AudienceScore")
                        .HasColumnType("float");

                    b.Property<DateTime>("Finale")
                        .HasColumnType("datetime2");

                    b.Property<int>("Index")
                        .HasColumnType("int");

                    b.Property<int>("MinutesPerEpisode")
                        .HasColumnType("int");

                    b.Property<DateTime>("Premier")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AnimeId");

                    b.ToTable("Seasons");
                });

            modelBuilder.Entity("AnimeStats.Service.Season", b =>
                {
                    b.HasOne("AnimeStats.Service.Anime", null)
                        .WithMany("Seasons")
                        .HasForeignKey("AnimeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AnimeStats.Service.Anime", b =>
                {
                    b.Navigation("Seasons");
                });
#pragma warning restore 612, 618
        }
    }
}
