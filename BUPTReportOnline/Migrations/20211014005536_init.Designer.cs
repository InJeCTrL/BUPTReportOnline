﻿// <auto-generated />
using BUPTReportOnline.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BUPTReportOnline.Migrations
{
    [DbContext(typeof(BROContext))]
    [Migration("20211014005536_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.11");

            modelBuilder.Entity("BUPTReportOnline.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Cookie")
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasColumnType("longtext");

                    b.Property<int>("EndHour")
                        .HasColumnType("int");

                    b.Property<int>("EndMinute")
                        .HasColumnType("int");

                    b.Property<string>("GUID")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("LastMessage")
                        .HasColumnType("longtext");

                    b.Property<bool>("LastResult")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("LastTime")
                        .HasColumnType("longtext");

                    b.Property<bool>("Registered")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("SendInform")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("StartHour")
                        .HasColumnType("int");

                    b.Property<int>("StartMinute")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("User");
                });
#pragma warning restore 612, 618
        }
    }
}
