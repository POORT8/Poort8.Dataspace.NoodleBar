﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Poort8.Dataspace.OrganizationRegistry;

#nullable disable

namespace Poort8.Dataspace.CoreManager.Migrations
{
    [DbContext(typeof(OrganizationContext))]
    partial class OrganizationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("Poort8.Dataspace.OrganizationRegistry.Agreement", b =>
                {
                    b.Property<string>("AgreementId")
                        .HasColumnType("TEXT");

                    b.Property<bool?>("CompliancyVerified")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("ContractFile")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<string>("DataspaceId")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("DateOfExpiry")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("DateOfSigning")
                        .HasColumnType("TEXT");

                    b.Property<string>("Framework")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("HashOfSignedContract")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("OrganizationIdentifier")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("AgreementId");

                    b.HasIndex("OrganizationIdentifier");

                    b.ToTable("Agreement");
                });

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

            modelBuilder.Entity("Poort8.Dataspace.OrganizationRegistry.AuthorizationRegistry", b =>
                {
                    b.Property<string>("AuthorizationRegistryId")
                        .HasColumnType("TEXT");

                    b.Property<string>("AuthorizationRegistryOrganizationId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("AuthorizationRegistryUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("DataspaceId")
                        .HasColumnType("TEXT");

                    b.Property<string>("OrganizationIdentifier")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("AuthorizationRegistryId");

                    b.HasIndex("OrganizationIdentifier");

                    b.ToTable("AuthorizationRegistry");
                });

            modelBuilder.Entity("Poort8.Dataspace.OrganizationRegistry.Certificate", b =>
                {
                    b.Property<string>("CertificateId")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("CertificateFile")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<DateOnly>("EnabledFrom")
                        .HasColumnType("TEXT");

                    b.Property<string>("OrganizationIdentifier")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("CertificateId");

                    b.HasIndex("OrganizationIdentifier");

                    b.ToTable("Certificate");
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

                    b.Property<bool>("CompliancyVerified")
                        .HasColumnType("INTEGER");

                    b.Property<DateOnly>("EndDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("LegalAdherence")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LoA")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("OrganizationIdentifier")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("StartDate")
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

            modelBuilder.Entity("Poort8.Dataspace.OrganizationRegistry.Service", b =>
                {
                    b.Property<string>("ServiceId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Color")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("LogoUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("OrganizationIdentifier")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.HasKey("ServiceId");

                    b.HasIndex("OrganizationIdentifier");

                    b.ToTable("Service");
                });

            modelBuilder.Entity("Poort8.Dataspace.OrganizationRegistry.Agreement", b =>
                {
                    b.HasOne("Poort8.Dataspace.OrganizationRegistry.Organization", null)
                        .WithMany("Agreements")
                        .HasForeignKey("OrganizationIdentifier")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Poort8.Dataspace.OrganizationRegistry.AuthorizationRegistry", b =>
                {
                    b.HasOne("Poort8.Dataspace.OrganizationRegistry.Organization", null)
                        .WithMany("AuthorizationRegistries")
                        .HasForeignKey("OrganizationIdentifier")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Poort8.Dataspace.OrganizationRegistry.Certificate", b =>
                {
                    b.HasOne("Poort8.Dataspace.OrganizationRegistry.Organization", null)
                        .WithMany("Certificates")
                        .HasForeignKey("OrganizationIdentifier")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Poort8.Dataspace.OrganizationRegistry.Organization", b =>
                {
                    b.OwnsOne("Poort8.Dataspace.OrganizationRegistry.AdditionalDetails", "AdditionalDetails", b1 =>
                        {
                            b1.Property<string>("OrganizationIdentifier")
                                .HasColumnType("TEXT");

                            b1.Property<string>("CapabilitiesUrl")
                                .HasColumnType("TEXT");

                            b1.Property<string>("CompanyEmail")
                                .HasColumnType("TEXT");

                            b1.Property<string>("CompanyPhone")
                                .HasColumnType("TEXT");

                            b1.Property<string>("CountriesOfOperation")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<string>("Description")
                                .HasColumnType("TEXT");

                            b1.Property<string>("LogoUrl")
                                .HasColumnType("TEXT");

                            b1.Property<bool?>("PubliclyPublishable")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Sectors")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<string>("Tags")
                                .HasColumnType("TEXT");

                            b1.Property<string>("WebsiteUrl")
                                .HasColumnType("TEXT");

                            b1.HasKey("OrganizationIdentifier");

                            b1.ToTable("OrOrganization");

                            b1.WithOwner()
                                .HasForeignKey("OrganizationIdentifier");
                        });

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

                    b.Navigation("AdditionalDetails")
                        .IsRequired();

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

            modelBuilder.Entity("Poort8.Dataspace.OrganizationRegistry.Service", b =>
                {
                    b.HasOne("Poort8.Dataspace.OrganizationRegistry.Organization", null)
                        .WithMany("Services")
                        .HasForeignKey("OrganizationIdentifier")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Poort8.Dataspace.OrganizationRegistry.Organization", b =>
                {
                    b.Navigation("Agreements");

                    b.Navigation("AuthorizationRegistries");

                    b.Navigation("Certificates");

                    b.Navigation("Properties");

                    b.Navigation("Roles");

                    b.Navigation("Services");
                });
#pragma warning restore 612, 618
        }
    }
}