﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NineChronicles.DataProvider.Store;

namespace NineChronicles.DataProvider.Executable.Migrations
{
    [DbContext(typeof(NineChroniclesContext))]
    [Migration("20220422032300_AddShopHistory")]
    partial class AddShopHistory
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.10");

            modelBuilder.Entity("NineChronicles.DataProvider.Store.Models.AbilityRankingModel", b =>
                {
                    b.Property<string>("AvatarAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("AgentAddress")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ArmorId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AvatarLevel")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Cp")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Ranking")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("TitleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("AvatarAddress");

                    b.ToTable("AbilityRankings");
                });

            modelBuilder.Entity("NineChronicles.DataProvider.Store.Models.AgentModel", b =>
                {
                    b.Property<string>("Address")
                        .HasColumnType("TEXT");

                    b.HasKey("Address");

                    b.ToTable("Agents");
                });

            modelBuilder.Entity("NineChronicles.DataProvider.Store.Models.AvatarModel", b =>
                {
                    b.Property<string>("Address")
                        .HasColumnType("TEXT");

                    b.Property<string>("AgentAddress")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ArmorId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AvatarLevel")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Cp")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("TitleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Address");

                    b.HasIndex("AgentAddress");

                    b.ToTable("Avatars");
                });

            modelBuilder.Entity("NineChronicles.DataProvider.Store.Models.CombinationConsumableModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("AgentAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("AvatarAddress")
                        .HasColumnType("TEXT");

                    b.Property<long>("BlockIndex")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RecipeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SlotIndex")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AgentAddress");

                    b.HasIndex("AvatarAddress");

                    b.ToTable("CombinationConsumables");
                });

            modelBuilder.Entity("NineChronicles.DataProvider.Store.Models.CombinationEquipmentModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("AgentAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("AvatarAddress")
                        .HasColumnType("TEXT");

                    b.Property<long>("BlockIndex")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RecipeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SlotIndex")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("SubRecipeId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AgentAddress");

                    b.HasIndex("AvatarAddress");

                    b.ToTable("CombinationEquipments");
                });

            modelBuilder.Entity("NineChronicles.DataProvider.Store.Models.CraftRankingInputModel", b =>
                {
                    b.Property<string>("AvatarAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("AgentAddress")
                        .HasColumnType("TEXT");

                    b.Property<long>("BlockIndex")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CraftCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Ranking")
                        .HasColumnType("INTEGER");

                    b.HasKey("AvatarAddress");

                    b.ToTable("CraftRankings");
                });

            modelBuilder.Entity("NineChronicles.DataProvider.Store.Models.CraftRankingOutputModel", b =>
                {
                    b.Property<string>("AvatarAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("AgentAddress")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ArmorId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AvatarLevel")
                        .HasColumnType("INTEGER");

                    b.Property<long>("BlockIndex")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Cp")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CraftCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Ranking")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("TitleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("AvatarAddress");

                    b.ToTable("CraftRankingsOutput");
                });

            modelBuilder.Entity("NineChronicles.DataProvider.Store.Models.EquipmentModel", b =>
                {
                    b.Property<string>("ItemId")
                        .HasColumnType("TEXT");

                    b.Property<string>("AgentAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("AvatarAddress")
                        .HasColumnType("TEXT");

                    b.Property<int>("Cp")
                        .HasColumnType("INTEGER");

                    b.Property<int>("EquipmentId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ItemSubType")
                        .HasColumnType("TEXT");

                    b.Property<int>("Level")
                        .HasColumnType("INTEGER");

                    b.HasKey("ItemId");

                    b.ToTable("Equipments");
                });

            modelBuilder.Entity("NineChronicles.DataProvider.Store.Models.EquipmentRankingModel", b =>
                {
                    b.Property<string>("ItemId")
                        .HasColumnType("TEXT");

                    b.Property<string>("AgentAddress")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ArmorId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("AvatarAddress")
                        .HasColumnType("TEXT");

                    b.Property<int?>("AvatarLevel")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Cp")
                        .HasColumnType("INTEGER");

                    b.Property<int>("EquipmentId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ItemSubType")
                        .HasColumnType("TEXT");

                    b.Property<int>("Level")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("Ranking")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("TitleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ItemId");

                    b.ToTable("EquipmentRankings");
                });

            modelBuilder.Entity("NineChronicles.DataProvider.Store.Models.HackAndSlashModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("AgentAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("AvatarAddress")
                        .HasColumnType("TEXT");

                    b.Property<long>("BlockIndex")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Cleared")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Mimisbrunnr")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StageId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AgentAddress");

                    b.HasIndex("AvatarAddress");

                    b.ToTable("HackAndSlashes");
                });

            modelBuilder.Entity("NineChronicles.DataProvider.Store.Models.ItemEnhancementModel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("AgentAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("AvatarAddress")
                        .HasColumnType("TEXT");

                    b.Property<long>("BlockIndex")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ItemId")
                        .HasColumnType("TEXT");

                    b.Property<string>("MaterialId")
                        .HasColumnType("TEXT");

                    b.Property<int>("SlotIndex")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AgentAddress");

                    b.HasIndex("AvatarAddress");

                    b.ToTable("ItemEnhancements");
                });

            modelBuilder.Entity("NineChronicles.DataProvider.Store.Models.ShopHistoryConsumableModel", b =>
                {
                    b.Property<string>("OrderId")
                        .HasColumnType("TEXT");

                    b.Property<string>("BlockHash")
                        .HasColumnType("TEXT");

                    b.Property<long>("BlockIndex")
                        .HasColumnType("INTEGER");

                    b.Property<int>("BuffSkillCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("BuyerAvatarAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("ElementalType")
                        .HasColumnType("TEXT");

                    b.Property<int>("Grade")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ItemCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ItemId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ItemSubType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ItemType")
                        .HasColumnType("TEXT");

                    b.Property<string>("MainStat")
                        .HasColumnType("TEXT");

                    b.Property<string>("NonFungibleId")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Price")
                        .HasColumnType("TEXT");

                    b.Property<long>("RequiredBlockIndex")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SellerAvatarAddress")
                        .HasColumnType("TEXT");

                    b.Property<int>("SkillsCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TimeStamp")
                        .HasColumnType("TEXT");

                    b.Property<string>("TradableId")
                        .HasColumnType("TEXT");

                    b.Property<string>("TxId")
                        .HasColumnType("TEXT");

                    b.HasKey("OrderId");

                    b.ToTable("ShopHistoryConsumables");
                });

            modelBuilder.Entity("NineChronicles.DataProvider.Store.Models.ShopHistoryCostumeModel", b =>
                {
                    b.Property<string>("OrderId")
                        .HasColumnType("TEXT");

                    b.Property<string>("BlockHash")
                        .HasColumnType("TEXT");

                    b.Property<long>("BlockIndex")
                        .HasColumnType("INTEGER");

                    b.Property<string>("BuyerAvatarAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("ElementalType")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Equipped")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Grade")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ItemCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ItemId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ItemSubType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ItemType")
                        .HasColumnType("TEXT");

                    b.Property<string>("NonFungibleId")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Price")
                        .HasColumnType("TEXT");

                    b.Property<long>("RequiredBlockIndex")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SellerAvatarAddress")
                        .HasColumnType("TEXT");

                    b.Property<int>("SetId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SpineResourcePath")
                        .HasColumnType("TEXT");

                    b.Property<string>("TimeStamp")
                        .HasColumnType("TEXT");

                    b.Property<string>("TradableId")
                        .HasColumnType("TEXT");

                    b.Property<string>("TxId")
                        .HasColumnType("TEXT");

                    b.HasKey("OrderId");

                    b.ToTable("ShopHistoryCostumes");
                });

            modelBuilder.Entity("NineChronicles.DataProvider.Store.Models.ShopHistoryEquipmentModel", b =>
                {
                    b.Property<string>("OrderId")
                        .HasColumnType("TEXT");

                    b.Property<string>("BlockHash")
                        .HasColumnType("TEXT");

                    b.Property<long>("BlockIndex")
                        .HasColumnType("INTEGER");

                    b.Property<int>("BuffSkillCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("BuyerAvatarAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("ElementalType")
                        .HasColumnType("TEXT");

                    b.Property<int>("Grade")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ItemCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ItemId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ItemSubType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ItemType")
                        .HasColumnType("TEXT");

                    b.Property<string>("NonFungibleId")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Price")
                        .HasColumnType("TEXT");

                    b.Property<long>("RequiredBlockIndex")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SellerAvatarAddress")
                        .HasColumnType("TEXT");

                    b.Property<int>("SetId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SkillsCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SpineResourcePath")
                        .HasColumnType("TEXT");

                    b.Property<string>("TimeStamp")
                        .HasColumnType("TEXT");

                    b.Property<string>("TradableId")
                        .HasColumnType("TEXT");

                    b.Property<string>("TxId")
                        .HasColumnType("TEXT");

                    b.Property<string>("UniqueStatType")
                        .HasColumnType("TEXT");

                    b.HasKey("OrderId");

                    b.ToTable("ShopHistoryEquipments");
                });

            modelBuilder.Entity("NineChronicles.DataProvider.Store.Models.ShopHistoryMaterialModel", b =>
                {
                    b.Property<string>("OrderId")
                        .HasColumnType("TEXT");

                    b.Property<string>("BlockHash")
                        .HasColumnType("TEXT");

                    b.Property<long>("BlockIndex")
                        .HasColumnType("INTEGER");

                    b.Property<string>("BuyerAvatarAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("ElementalType")
                        .HasColumnType("TEXT");

                    b.Property<int>("Grade")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ItemCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ItemId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ItemSubType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ItemType")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Price")
                        .HasColumnType("TEXT");

                    b.Property<string>("SellerAvatarAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("TimeStamp")
                        .HasColumnType("TEXT");

                    b.Property<string>("TxId")
                        .HasColumnType("TEXT");

                    b.HasKey("OrderId");

                    b.ToTable("ShopHistoryMaterials");
                });

            modelBuilder.Entity("NineChronicles.DataProvider.Store.Models.StageRankingModel", b =>
                {
                    b.Property<int>("Ranking")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AgentAddress")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ArmorId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("AvatarAddress")
                        .HasColumnType("TEXT");

                    b.Property<int?>("AvatarLevel")
                        .HasColumnType("INTEGER");

                    b.Property<long>("BlockIndex")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ClearedStageId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Cp")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("TitleId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Ranking");

                    b.ToTable("StageRanking");
                });

            modelBuilder.Entity("NineChronicles.DataProvider.Store.Models.AvatarModel", b =>
                {
                    b.HasOne("NineChronicles.DataProvider.Store.Models.AgentModel", "Agent")
                        .WithMany()
                        .HasForeignKey("AgentAddress");

                    b.Navigation("Agent");
                });

            modelBuilder.Entity("NineChronicles.DataProvider.Store.Models.CombinationConsumableModel", b =>
                {
                    b.HasOne("NineChronicles.DataProvider.Store.Models.AgentModel", "Agent")
                        .WithMany()
                        .HasForeignKey("AgentAddress");

                    b.HasOne("NineChronicles.DataProvider.Store.Models.AvatarModel", "Avatar")
                        .WithMany()
                        .HasForeignKey("AvatarAddress");

                    b.Navigation("Agent");

                    b.Navigation("Avatar");
                });

            modelBuilder.Entity("NineChronicles.DataProvider.Store.Models.CombinationEquipmentModel", b =>
                {
                    b.HasOne("NineChronicles.DataProvider.Store.Models.AgentModel", "Agent")
                        .WithMany()
                        .HasForeignKey("AgentAddress");

                    b.HasOne("NineChronicles.DataProvider.Store.Models.AvatarModel", "Avatar")
                        .WithMany()
                        .HasForeignKey("AvatarAddress");

                    b.Navigation("Agent");

                    b.Navigation("Avatar");
                });

            modelBuilder.Entity("NineChronicles.DataProvider.Store.Models.HackAndSlashModel", b =>
                {
                    b.HasOne("NineChronicles.DataProvider.Store.Models.AgentModel", "Agent")
                        .WithMany()
                        .HasForeignKey("AgentAddress");

                    b.HasOne("NineChronicles.DataProvider.Store.Models.AvatarModel", "Avatar")
                        .WithMany()
                        .HasForeignKey("AvatarAddress");

                    b.Navigation("Agent");

                    b.Navigation("Avatar");
                });

            modelBuilder.Entity("NineChronicles.DataProvider.Store.Models.ItemEnhancementModel", b =>
                {
                    b.HasOne("NineChronicles.DataProvider.Store.Models.AgentModel", "Agent")
                        .WithMany()
                        .HasForeignKey("AgentAddress");

                    b.HasOne("NineChronicles.DataProvider.Store.Models.AvatarModel", "Avatar")
                        .WithMany()
                        .HasForeignKey("AvatarAddress");

                    b.Navigation("Agent");

                    b.Navigation("Avatar");
                });
#pragma warning restore 612, 618
        }
    }
}