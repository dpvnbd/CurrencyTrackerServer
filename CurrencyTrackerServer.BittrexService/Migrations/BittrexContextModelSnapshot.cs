using System;
using CurrencyTrackerServer.ChangeTrackerService.Concrete.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CurrencyTrackerServer.ChangeTrackerService.Migrations
{
    [DbContext(typeof(BittrexContext))]
    partial class BittrexContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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
