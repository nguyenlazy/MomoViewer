using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using MomoViewer.Repository.DataAcess;

namespace MomoViewer.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20161005164107_SQLiteMigration")]
    partial class SQLiteMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1");

            modelBuilder.Entity("MomoViewer.Model.LinkInfo", b =>
                {
                    b.Property<int>("LinkInfoId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Path");

                    b.Property<int>("Type");

                    b.HasKey("LinkInfoId");

                    b.ToTable("Links");
                });
        }
    }
}
