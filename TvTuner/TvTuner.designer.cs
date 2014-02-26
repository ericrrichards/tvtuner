﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34003
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TvTuner
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="TvTuner")]
	public partial class TvTunerDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertEpisode(Episode instance);
    partial void UpdateEpisode(Episode instance);
    partial void DeleteEpisode(Episode instance);
    partial void InsertMovies(Movies instance);
    partial void UpdateMovies(Movies instance);
    partial void DeleteMovies(Movies instance);
    partial void InsertSeries(Series instance);
    partial void UpdateSeries(Series instance);
    partial void DeleteSeries(Series instance);
    partial void InsertCategory(Category instance);
    partial void UpdateCategory(Category instance);
    partial void DeleteCategory(Category instance);
    partial void InsertChannel(Channel instance);
    partial void UpdateChannel(Channel instance);
    partial void DeleteChannel(Channel instance);
    partial void InsertChannelSery(ChannelSery instance);
    partial void UpdateChannelSery(ChannelSery instance);
    partial void DeleteChannelSery(ChannelSery instance);
    #endregion
		
		public TvTunerDataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["TvTunerConnectionString2"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public TvTunerDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public TvTunerDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public TvTunerDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public TvTunerDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Episode> Episodes
		{
			get
			{
				return this.GetTable<Episode>();
			}
		}
		
		public System.Data.Linq.Table<Movies> Movies
		{
			get
			{
				return this.GetTable<Movies>();
			}
		}
		
		public System.Data.Linq.Table<Series> Series
		{
			get
			{
				return this.GetTable<Series>();
			}
		}
		
		public System.Data.Linq.Table<Category> Categories
		{
			get
			{
				return this.GetTable<Category>();
			}
		}
		
		public System.Data.Linq.Table<Channel> Channels
		{
			get
			{
				return this.GetTable<Channel>();
			}
		}
		
		public System.Data.Linq.Table<ChannelSery> ChannelSeries
		{
			get
			{
				return this.GetTable<ChannelSery>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Episodes")]
	public partial class Episode : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _EpisodeID;
		
		private int _SeriesID;
		
		private int _Season;
		
		private int _EpisodeNumber;
		
		private string _Name;
		
		private string _Summary;
		
		private System.Data.Linq.Binary _Thumbnail;
		
		private string _VideoPath;
		
		private EntityRef<Series> _Series;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnEpisodeIDChanging(int value);
    partial void OnEpisodeIDChanged();
    partial void OnSeriesIDChanging(int value);
    partial void OnSeriesIDChanged();
    partial void OnSeasonChanging(int value);
    partial void OnSeasonChanged();
    partial void OnEpisodeNumberChanging(int value);
    partial void OnEpisodeNumberChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnSummaryChanging(string value);
    partial void OnSummaryChanged();
    partial void OnThumbnailChanging(System.Data.Linq.Binary value);
    partial void OnThumbnailChanged();
    partial void OnVideoPathChanging(string value);
    partial void OnVideoPathChanged();
    #endregion
		
		public Episode()
		{
			this._Series = default(EntityRef<Series>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EpisodeID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int EpisodeID
		{
			get
			{
				return this._EpisodeID;
			}
			set
			{
				if ((this._EpisodeID != value))
				{
					this.OnEpisodeIDChanging(value);
					this.SendPropertyChanging();
					this._EpisodeID = value;
					this.SendPropertyChanged("EpisodeID");
					this.OnEpisodeIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SeriesID", DbType="Int NOT NULL")]
		public int SeriesID
		{
			get
			{
				return this._SeriesID;
			}
			set
			{
				if ((this._SeriesID != value))
				{
					if (this._Series.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnSeriesIDChanging(value);
					this.SendPropertyChanging();
					this._SeriesID = value;
					this.SendPropertyChanged("SeriesID");
					this.OnSeriesIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Season", DbType="Int NOT NULL")]
		public int Season
		{
			get
			{
				return this._Season;
			}
			set
			{
				if ((this._Season != value))
				{
					this.OnSeasonChanging(value);
					this.SendPropertyChanging();
					this._Season = value;
					this.SendPropertyChanged("Season");
					this.OnSeasonChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EpisodeNumber", DbType="Int NOT NULL")]
		public int EpisodeNumber
		{
			get
			{
				return this._EpisodeNumber;
			}
			set
			{
				if ((this._EpisodeNumber != value))
				{
					this.OnEpisodeNumberChanging(value);
					this.SendPropertyChanging();
					this._EpisodeNumber = value;
					this.SendPropertyChanged("EpisodeNumber");
					this.OnEpisodeNumberChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(200) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Summary", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string Summary
		{
			get
			{
				return this._Summary;
			}
			set
			{
				if ((this._Summary != value))
				{
					this.OnSummaryChanging(value);
					this.SendPropertyChanging();
					this._Summary = value;
					this.SendPropertyChanged("Summary");
					this.OnSummaryChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Thumbnail", DbType="VarBinary(MAX) NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public System.Data.Linq.Binary Thumbnail
		{
			get
			{
				return this._Thumbnail;
			}
			set
			{
				if ((this._Thumbnail != value))
				{
					this.OnThumbnailChanging(value);
					this.SendPropertyChanging();
					this._Thumbnail = value;
					this.SendPropertyChanged("Thumbnail");
					this.OnThumbnailChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_VideoPath", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string VideoPath
		{
			get
			{
				return this._VideoPath;
			}
			set
			{
				if ((this._VideoPath != value))
				{
					this.OnVideoPathChanging(value);
					this.SendPropertyChanging();
					this._VideoPath = value;
					this.SendPropertyChanged("VideoPath");
					this.OnVideoPathChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Series_Episode", Storage="_Series", ThisKey="SeriesID", OtherKey="SeriesID", IsForeignKey=true)]
		public Series Series
		{
			get
			{
				return this._Series.Entity;
			}
			set
			{
				Series previousValue = this._Series.Entity;
				if (((previousValue != value) 
							|| (this._Series.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Series.Entity = null;
						previousValue.Episodes.Remove(this);
					}
					this._Series.Entity = value;
					if ((value != null))
					{
						value.Episodes.Add(this);
						this._SeriesID = value.SeriesID;
					}
					else
					{
						this._SeriesID = default(int);
					}
					this.SendPropertyChanged("Series");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Movies")]
	public partial class Movies : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _MovieID;
		
		private string _Title;
		
		private int _Year;
		
		private string _Summary;
		
		private int _Rating;
		
		private System.Data.Linq.Binary _Thumbnail;
		
		private string _VideoPath;
		
		private string _Genre;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnMovieIDChanging(int value);
    partial void OnMovieIDChanged();
    partial void OnTitleChanging(string value);
    partial void OnTitleChanged();
    partial void OnYearChanging(int value);
    partial void OnYearChanged();
    partial void OnSummaryChanging(string value);
    partial void OnSummaryChanged();
    partial void OnRatingChanging(int value);
    partial void OnRatingChanged();
    partial void OnThumbnailChanging(System.Data.Linq.Binary value);
    partial void OnThumbnailChanged();
    partial void OnVideoPathChanging(string value);
    partial void OnVideoPathChanged();
    partial void OnGenreChanging(string value);
    partial void OnGenreChanged();
    #endregion
		
		public Movies()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_MovieID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int MovieID
		{
			get
			{
				return this._MovieID;
			}
			set
			{
				if ((this._MovieID != value))
				{
					this.OnMovieIDChanging(value);
					this.SendPropertyChanging();
					this._MovieID = value;
					this.SendPropertyChanged("MovieID");
					this.OnMovieIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Title", DbType="NVarChar(200) NOT NULL", CanBeNull=false)]
		public string Title
		{
			get
			{
				return this._Title;
			}
			set
			{
				if ((this._Title != value))
				{
					this.OnTitleChanging(value);
					this.SendPropertyChanging();
					this._Title = value;
					this.SendPropertyChanged("Title");
					this.OnTitleChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Year", DbType="Int NOT NULL")]
		public int Year
		{
			get
			{
				return this._Year;
			}
			set
			{
				if ((this._Year != value))
				{
					this.OnYearChanging(value);
					this.SendPropertyChanging();
					this._Year = value;
					this.SendPropertyChanged("Year");
					this.OnYearChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Summary", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string Summary
		{
			get
			{
				return this._Summary;
			}
			set
			{
				if ((this._Summary != value))
				{
					this.OnSummaryChanging(value);
					this.SendPropertyChanging();
					this._Summary = value;
					this.SendPropertyChanged("Summary");
					this.OnSummaryChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Rating", DbType="Int NOT NULL")]
		public int Rating
		{
			get
			{
				return this._Rating;
			}
			set
			{
				if ((this._Rating != value))
				{
					this.OnRatingChanging(value);
					this.SendPropertyChanging();
					this._Rating = value;
					this.SendPropertyChanged("Rating");
					this.OnRatingChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Thumbnail", DbType="VarBinary(MAX) NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public System.Data.Linq.Binary Thumbnail
		{
			get
			{
				return this._Thumbnail;
			}
			set
			{
				if ((this._Thumbnail != value))
				{
					this.OnThumbnailChanging(value);
					this.SendPropertyChanging();
					this._Thumbnail = value;
					this.SendPropertyChanged("Thumbnail");
					this.OnThumbnailChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_VideoPath", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string VideoPath
		{
			get
			{
				return this._VideoPath;
			}
			set
			{
				if ((this._VideoPath != value))
				{
					this.OnVideoPathChanging(value);
					this.SendPropertyChanging();
					this._VideoPath = value;
					this.SendPropertyChanged("VideoPath");
					this.OnVideoPathChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Genre", DbType="NVarChar(200)")]
		public string Genre
		{
			get
			{
				return this._Genre;
			}
			set
			{
				if ((this._Genre != value))
				{
					this.OnGenreChanging(value);
					this.SendPropertyChanging();
					this._Genre = value;
					this.SendPropertyChanged("Genre");
					this.OnGenreChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Series")]
	public partial class Series : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _SeriesID;
		
		private string _Name;
		
		private System.Data.Linq.Binary _BannerImg;
		
		private string _Summary;
		
		private System.Nullable<int> _CategoryID;
		
		private EntitySet<Episode> _Episodes;
		
		private EntityRef<ChannelSery> _ChannelSery;
		
		private EntityRef<Category> _Category;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnSeriesIDChanging(int value);
    partial void OnSeriesIDChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnBannerImgChanging(System.Data.Linq.Binary value);
    partial void OnBannerImgChanged();
    partial void OnSummaryChanging(string value);
    partial void OnSummaryChanged();
    partial void OnCategoryIDChanging(System.Nullable<int> value);
    partial void OnCategoryIDChanged();
    #endregion
		
		public Series()
		{
			this._Episodes = new EntitySet<Episode>(new Action<Episode>(this.attach_Episodes), new Action<Episode>(this.detach_Episodes));
			this._ChannelSery = default(EntityRef<ChannelSery>);
			this._Category = default(EntityRef<Category>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SeriesID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int SeriesID
		{
			get
			{
				return this._SeriesID;
			}
			set
			{
				if ((this._SeriesID != value))
				{
					this.OnSeriesIDChanging(value);
					this.SendPropertyChanging();
					this._SeriesID = value;
					this.SendPropertyChanged("SeriesID");
					this.OnSeriesIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_BannerImg", DbType="VarBinary(MAX) NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public System.Data.Linq.Binary BannerImg
		{
			get
			{
				return this._BannerImg;
			}
			set
			{
				if ((this._BannerImg != value))
				{
					this.OnBannerImgChanging(value);
					this.SendPropertyChanging();
					this._BannerImg = value;
					this.SendPropertyChanged("BannerImg");
					this.OnBannerImgChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Summary", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string Summary
		{
			get
			{
				return this._Summary;
			}
			set
			{
				if ((this._Summary != value))
				{
					this.OnSummaryChanging(value);
					this.SendPropertyChanging();
					this._Summary = value;
					this.SendPropertyChanged("Summary");
					this.OnSummaryChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CategoryID", DbType="Int")]
		public System.Nullable<int> CategoryID
		{
			get
			{
				return this._CategoryID;
			}
			set
			{
				if ((this._CategoryID != value))
				{
					if (this._Category.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnCategoryIDChanging(value);
					this.SendPropertyChanging();
					this._CategoryID = value;
					this.SendPropertyChanged("CategoryID");
					this.OnCategoryIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Series_Episode", Storage="_Episodes", ThisKey="SeriesID", OtherKey="SeriesID")]
		public EntitySet<Episode> Episodes
		{
			get
			{
				return this._Episodes;
			}
			set
			{
				this._Episodes.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Series_ChannelSery", Storage="_ChannelSery", ThisKey="SeriesID", OtherKey="SeriesID", IsUnique=true, IsForeignKey=false)]
		public ChannelSery ChannelSery
		{
			get
			{
				return this._ChannelSery.Entity;
			}
			set
			{
				ChannelSery previousValue = this._ChannelSery.Entity;
				if (((previousValue != value) 
							|| (this._ChannelSery.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._ChannelSery.Entity = null;
						previousValue.Series = null;
					}
					this._ChannelSery.Entity = value;
					if ((value != null))
					{
						value.Series = this;
					}
					this.SendPropertyChanged("ChannelSery");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Category_Series", Storage="_Category", ThisKey="CategoryID", OtherKey="CategoryID", IsForeignKey=true)]
		public Category Category
		{
			get
			{
				return this._Category.Entity;
			}
			set
			{
				Category previousValue = this._Category.Entity;
				if (((previousValue != value) 
							|| (this._Category.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Category.Entity = null;
						previousValue.Series.Remove(this);
					}
					this._Category.Entity = value;
					if ((value != null))
					{
						value.Series.Add(this);
						this._CategoryID = value.CategoryID;
					}
					else
					{
						this._CategoryID = default(Nullable<int>);
					}
					this.SendPropertyChanged("Category");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Episodes(Episode entity)
		{
			this.SendPropertyChanging();
			entity.Series = this;
		}
		
		private void detach_Episodes(Episode entity)
		{
			this.SendPropertyChanging();
			entity.Series = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Categories")]
	public partial class Category : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _CategoryID;
		
		private string _CategoryName;
		
		private EntitySet<Series> _Series;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnCategoryIDChanging(int value);
    partial void OnCategoryIDChanged();
    partial void OnCategoryNameChanging(string value);
    partial void OnCategoryNameChanged();
    #endregion
		
		public Category()
		{
			this._Series = new EntitySet<Series>(new Action<Series>(this.attach_Series), new Action<Series>(this.detach_Series));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CategoryID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int CategoryID
		{
			get
			{
				return this._CategoryID;
			}
			set
			{
				if ((this._CategoryID != value))
				{
					this.OnCategoryIDChanging(value);
					this.SendPropertyChanging();
					this._CategoryID = value;
					this.SendPropertyChanged("CategoryID");
					this.OnCategoryIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CategoryName", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string CategoryName
		{
			get
			{
				return this._CategoryName;
			}
			set
			{
				if ((this._CategoryName != value))
				{
					this.OnCategoryNameChanging(value);
					this.SendPropertyChanging();
					this._CategoryName = value;
					this.SendPropertyChanged("CategoryName");
					this.OnCategoryNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Category_Series", Storage="_Series", ThisKey="CategoryID", OtherKey="CategoryID")]
		public EntitySet<Series> Series
		{
			get
			{
				return this._Series;
			}
			set
			{
				this._Series.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Series(Series entity)
		{
			this.SendPropertyChanging();
			entity.Category = this;
		}
		
		private void detach_Series(Series entity)
		{
			this.SendPropertyChanging();
			entity.Category = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Channels")]
	public partial class Channel : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _ChannelID;
		
		private string _Name;
		
		private EntitySet<ChannelSery> _ChannelSeries;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnChannelIDChanging(int value);
    partial void OnChannelIDChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    #endregion
		
		public Channel()
		{
			this._ChannelSeries = new EntitySet<ChannelSery>(new Action<ChannelSery>(this.attach_ChannelSeries), new Action<ChannelSery>(this.detach_ChannelSeries));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ChannelID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ChannelID
		{
			get
			{
				return this._ChannelID;
			}
			set
			{
				if ((this._ChannelID != value))
				{
					this.OnChannelIDChanging(value);
					this.SendPropertyChanging();
					this._ChannelID = value;
					this.SendPropertyChanged("ChannelID");
					this.OnChannelIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Channel_ChannelSery", Storage="_ChannelSeries", ThisKey="ChannelID", OtherKey="ChannelID")]
		public EntitySet<ChannelSery> ChannelSeries
		{
			get
			{
				return this._ChannelSeries;
			}
			set
			{
				this._ChannelSeries.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_ChannelSeries(ChannelSery entity)
		{
			this.SendPropertyChanging();
			entity.Channel = this;
		}
		
		private void detach_ChannelSeries(ChannelSery entity)
		{
			this.SendPropertyChanging();
			entity.Channel = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.ChannelSeries")]
	public partial class ChannelSery : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _ID;
		
		private int _ChannelID;
		
		private int _SeriesID;
		
		private EntityRef<Channel> _Channel;
		
		private EntityRef<Series> _Series;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(int value);
    partial void OnIDChanged();
    partial void OnChannelIDChanging(int value);
    partial void OnChannelIDChanged();
    partial void OnSeriesIDChanging(int value);
    partial void OnSeriesIDChanged();
    #endregion
		
		public ChannelSery()
		{
			this._Channel = default(EntityRef<Channel>);
			this._Series = default(EntityRef<Series>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ChannelID", DbType="Int NOT NULL")]
		public int ChannelID
		{
			get
			{
				return this._ChannelID;
			}
			set
			{
				if ((this._ChannelID != value))
				{
					if (this._Channel.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnChannelIDChanging(value);
					this.SendPropertyChanging();
					this._ChannelID = value;
					this.SendPropertyChanged("ChannelID");
					this.OnChannelIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SeriesID", DbType="Int NOT NULL")]
		public int SeriesID
		{
			get
			{
				return this._SeriesID;
			}
			set
			{
				if ((this._SeriesID != value))
				{
					if (this._Series.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnSeriesIDChanging(value);
					this.SendPropertyChanging();
					this._SeriesID = value;
					this.SendPropertyChanged("SeriesID");
					this.OnSeriesIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Channel_ChannelSery", Storage="_Channel", ThisKey="ChannelID", OtherKey="ChannelID", IsForeignKey=true)]
		public Channel Channel
		{
			get
			{
				return this._Channel.Entity;
			}
			set
			{
				Channel previousValue = this._Channel.Entity;
				if (((previousValue != value) 
							|| (this._Channel.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Channel.Entity = null;
						previousValue.ChannelSeries.Remove(this);
					}
					this._Channel.Entity = value;
					if ((value != null))
					{
						value.ChannelSeries.Add(this);
						this._ChannelID = value.ChannelID;
					}
					else
					{
						this._ChannelID = default(int);
					}
					this.SendPropertyChanged("Channel");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Series_ChannelSery", Storage="_Series", ThisKey="SeriesID", OtherKey="SeriesID", IsForeignKey=true)]
		public Series Series
		{
			get
			{
				return this._Series.Entity;
			}
			set
			{
				Series previousValue = this._Series.Entity;
				if (((previousValue != value) 
							|| (this._Series.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Series.Entity = null;
						previousValue.ChannelSery = null;
					}
					this._Series.Entity = value;
					if ((value != null))
					{
						value.ChannelSery = this;
						this._SeriesID = value.SeriesID;
					}
					else
					{
						this._SeriesID = default(int);
					}
					this.SendPropertyChanged("Series");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
