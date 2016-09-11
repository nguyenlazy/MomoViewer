using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using MomoViewer.Repository.DataAcess;

namespace MomoViewer.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20160911152710_MyFirstMigration")]
    partial class MyFirstMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("MomoViewer.Model.LinkInfo", b =>
                {
                    b.Property<int>("LinkInfoId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Path");

                    b.Property<int>("Type");

                    b.HasKey("LinkInfoId");

                    b.ToTable("Links");
                });
        }
    }
}
