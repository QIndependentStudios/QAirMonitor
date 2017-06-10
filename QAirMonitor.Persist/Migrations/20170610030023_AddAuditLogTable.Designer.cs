using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using QAirMonitor.Persist.Context;
using QAirMonitor.Domain.Enums;

namespace QAirMonitor.Persist.Migrations
{
    [DbContext(typeof(AppDataContext))]
    [Migration("20170610030023_AddAuditLogTable")]
    partial class AddAuditLogTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("QAirMonitor.Domain.Models.AuditLogModel", b =>
                {
                    b.Property<int>("AuditLogID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("EventDateTime");

                    b.Property<int>("EventType");

                    b.Property<string>("Message")
                        .HasMaxLength(2000);

                    b.HasKey("AuditLogID");

                    b.ToTable("AuditLog");
                });

            modelBuilder.Entity("QAirMonitor.Domain.Models.ReadingModel", b =>
                {
                    b.Property<int>("ReadingID")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Humidity");

                    b.Property<DateTime>("ReadingDateTime");

                    b.Property<double>("Temperature");

                    b.HasKey("ReadingID");

                    b.ToTable("Readings");
                });
        }
    }
}
