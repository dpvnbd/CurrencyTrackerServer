using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Data;
using CurrencyTrackerServer.ChangeTrackerService.Entities;

namespace CurrencyTrackerServer.ChangeTrackerService.Migrations
{
    [DbContext(typeof(ChangeTrackerContext))]
    partial class ChangeTrackerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("CurrencyTrackerServer.ChangeTrackerService.Entities.ChangeHistoryEntryEntity", b =>
                {
                    b.Property<string>("Currency");

                    b.Property<int>("ChangeSource");

                    b.Property<int>("Id");

                    b.Property<string>("Message");

                    b.Property<double>("Percentage");

                    b.Property<DateTime>("Time");

                    b.Property<int>("Type");

                    b.HasKey("Currency", "ChangeSource");

                    b.ToTable("History");
                });

            modelBuilder.Entity("CurrencyTrackerServer.ChangeTrackerService.Entities.CurrencyStateEntity", b =>
                {
                    b.Property<string>("Currency");

                    b.Property<int>("ChangeSource");

                    b.Property<DateTime>("Created");

                    b.Property<DateTime>("LastChangeTime");

                    b.Property<double>("Threshold");

                    b.HasKey("Currency", "ChangeSource");

                    b.ToTable("States");
                });
        }
    }
}
