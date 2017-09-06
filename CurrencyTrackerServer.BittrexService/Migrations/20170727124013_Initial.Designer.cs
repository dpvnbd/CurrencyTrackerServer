using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using CurrencyTrackerServer.ChangeTrackerService.Concrete;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Data;

namespace CurrencyTrackerServer.ChangeTrackerService.Migrations
{
    [DbContext(typeof(BittrexContext))]
    [Migration("20170727124013_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("CurrencyTrackerServer.BittrexService.Entities.ChangeHistoryEntryEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Currency");

                    b.Property<string>("Message");

                    b.Property<double>("Percentage");

                    b.Property<DateTime>("Time");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.ToTable("History");
                });

            modelBuilder.Entity("CurrencyTrackerServer.BittrexService.Entities.CurrencyStateEntity", b =>
                {
                    b.Property<string>("Currency")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<DateTime>("LastChangeTime");

                    b.Property<double>("Threshold");

                    b.HasKey("Currency");

                    b.ToTable("States");
                });
        }
    }
}
