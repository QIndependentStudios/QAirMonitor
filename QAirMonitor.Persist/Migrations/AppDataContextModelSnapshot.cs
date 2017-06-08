using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using QAirMonitor.Persist.Context;

namespace QAirMonitor.Persist.Migrations
{
    [DbContext(typeof(AppDataContext))]
    partial class AppDataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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
