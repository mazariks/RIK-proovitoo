﻿// <auto-generated />
using System;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20220301134925_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Event", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AdditionalInfo")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Naming")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Place")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("StartsAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("Domain.EventParticipant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<Guid>("ParticipantId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("ParticipantId");

                    b.ToTable("EventParticipants");
                });

            modelBuilder.Entity("Domain.Participant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AdditionalInfo")
                        .HasMaxLength(5000)
                        .HasColumnType("character varying(5000)");

                    b.Property<int?>("AmountOfGuests")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("text")
                        .HasComputedColumnSql("\"FirstName\" || ', ' || \"LastName\"", true);

                    b.Property<string>("IdentityCode")
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("Naming")
                        .HasColumnType("text");

                    b.Property<int>("PaymentType")
                        .HasColumnType("integer");

                    b.Property<string>("RegisterCode")
                        .HasColumnType("text");

                    b.Property<char>("Type")
                        .HasColumnType("character(1)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Participants");
                });

            modelBuilder.Entity("Domain.EventParticipant", b =>
                {
                    b.HasOne("Domain.Event", "Event")
                        .WithMany("EventParticipants")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Participant", "Participant")
                        .WithMany("EventParticipants")
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("Participant");
                });

            modelBuilder.Entity("Domain.Event", b =>
                {
                    b.Navigation("EventParticipants");
                });

            modelBuilder.Entity("Domain.Participant", b =>
                {
                    b.Navigation("EventParticipants");
                });
#pragma warning restore 612, 618
        }
    }
}
