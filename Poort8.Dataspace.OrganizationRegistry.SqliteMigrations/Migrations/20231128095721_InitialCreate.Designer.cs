﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Poort8.Dataspace.OrganizationRegistry;

#nullable disable

namespace Poort8.Dataspace.CoreManager.Migrations
{
    [DbContext(typeof(OrganizationContext))]
    [Migration("20231128095721_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("Poort8.Dataspace.OrganizationRegistry.AuditRecord", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Entity")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("EntityId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("EntityType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.Property<string>("User")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AuditRecords");
                });

            modelBuilder.Entity("Poort8.Dataspace.OrganizationRegistry.Organization", b =>
                {
                    b.Property<string>("Identifier")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Identifier");

                    b.ToTable("OrOrganization", (string)null);
                });

            modelBuilder.Entity("Poort8.Dataspace.OrganizationRegistry.OrganizationRole", b =>
                {
                    b.Property<string>("RoleId")
                        .HasColumnType("TEXT");

                    b.Property<string>("OrganizationIdentifier")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("RoleId");

                    b.HasIndex("OrganizationIdentifier");

                    b.ToTable("OrganizationRole");
                });

            modelBuilder.Entity("Poort8.Dataspace.OrganizationRegistry.Property", b =>
                {
                    b.Property<string>("PropertyId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsIdentifier")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("OrganizationIdentifier")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("PropertyId");

                    b.HasIndex("OrganizationIdentifier");

                    b.ToTable("Property");
                });

            modelBuilder.Entity("Poort8.Dataspace.OrganizationRegistry.Organization", b =>
                {
                    b.OwnsOne("Poort8.Dataspace.OrganizationRegistry.Adherence", "Adherence", b1 =>
                        {
                            b1.Property<string>("OrganizationIdentifier")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Status")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<DateOnly>("ValidFrom")
                                .HasColumnType("TEXT");

                            b1.Property<DateOnly>("ValidUntil")
                                .HasColumnType("TEXT");

                            b1.HasKey("OrganizationIdentifier");

                            b1.ToTable("OrOrganization");

                            b1.WithOwner()
                                .HasForeignKey("OrganizationIdentifier");
                        });

                    b.Navigation("Adherence")
                        .IsRequired();
                });

            modelBuilder.Entity("Poort8.Dataspace.OrganizationRegistry.OrganizationRole", b =>
                {
                    b.HasOne("Poort8.Dataspace.OrganizationRegistry.Organization", null)
                        .WithMany("Roles")
                        .HasForeignKey("OrganizationIdentifier")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Poort8.Dataspace.OrganizationRegistry.Property", b =>
                {
                    b.HasOne("Poort8.Dataspace.OrganizationRegistry.Organization", null)
                        .WithMany("Properties")
                        .HasForeignKey("OrganizationIdentifier")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Poort8.Dataspace.OrganizationRegistry.Organization", b =>
                {
                    b.Navigation("Properties");

                    b.Navigation("Roles");
                });
#pragma warning restore 612, 618
        }
    }
}