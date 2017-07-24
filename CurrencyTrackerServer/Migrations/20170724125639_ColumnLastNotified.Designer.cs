using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using CurrencyTrackerServer.Data;

namespace CurrencyTrackerServer.Migrations
{
    [DbContext(typeof(BittrexContext))]
    [Migration("20170724125639_ColumnLastNotified")]
    partial class ColumnLastNotified
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("CurrencyTrackerServer.Infrastructure.Entities.BittrexChange", b =>
                {
                    b.Property<string>("Currency")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ChangeTime");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<double>("CurrentBid");

                    b.Property<DateTime?>("LastNotifiedChange");

                    b.Property<double>("PreviousDayBid");

                    b.Property<string>("ReferenceCurrency");

                    b.Property<int>("Threshsold");

                    b.HasKey("Currency");

                    b.ToTable("Changes");
                });
        }
    }
}
