using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using QAirMonitor.Persist.Context;

namespace QAirMonitor.Persist.Migrations
{
    [DbContext(typeof(AppDataContext))]
    [Migration("20170608005723_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

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
