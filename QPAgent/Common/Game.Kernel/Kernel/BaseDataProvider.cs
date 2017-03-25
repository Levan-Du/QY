﻿namespace Game.Kernel
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;

    /// <summary>
    /// 数据访问基类
    /// </summary>
    public abstract class BaseDataProvider //: BaseProvider
    {
        private string m_connectionString;
        /// <summary>
        /// 连接字符串
        /// </summary>
        protected internal string ConnectionString
        {
            get
            {
                return this.m_connectionString;
            }
        }
        private DbHelper m_database;
        /// <summary>
        /// 数据库操作
        /// </summary>
        protected internal DbHelper Database
        {
            get
            {
                return this.m_database;
            }
        }
        private PagerManager m_pagerHelper;
        /// <summary>
        /// 分页对象
        /// </summary>
        protected internal PagerManager PagerHelper
        {
            get
            {
                return this.m_pagerHelper;
            }
        }

        protected internal BaseDataProvider( )
        {
        }
        /// <summary>
        /// 初始化数据访问基类对象
        /// </summary>
        /// <param name="database"></param>
        protected internal BaseDataProvider( DbHelper database )
        {
            this.m_database = database;
            this.m_connectionString = database.ConnectionString;
            this.m_pagerHelper = new PagerManager( this.m_database );
        }
        /// <summary>
        /// 初始化数据访问基类对象
        /// </summary>
        /// <param name="connectionString"></param>
        protected internal BaseDataProvider( string connectionString )
        {
            this.m_connectionString = connectionString;
            this.m_database = new DbHelper( connectionString );
            this.m_pagerHelper = new PagerManager( this.m_database );
        }
        /// <summary>
        /// 获取分页数据 排序字段仅一个
        /// </summary>
        /// <param name="prams"></param>
        /// <returns></returns>
        protected virtual PagerSet GetPagerSet( PagerParameters prams )
        {
            return this.PagerHelper.GetPagerSet( prams );
        }
        /// <summary>
        /// 获取分页数据 排序字段可以多个，需要自定义：Order by UserID DESC,Accounts ASC
        /// </summary>
        /// <param name="prams"></param>
        /// <returns></returns>
        protected virtual PagerSet GetPagerSet2( PagerParameters prams )
        {
            return this.PagerHelper.GetPagerSet2( prams );
        }
        /// <summary>
        /// 获取ITableProvider 实例对象
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        protected virtual ITableProvider GetTableProvider( string tableName )
        {
            return new TableProvider( this.Database , tableName );
        }

        protected PagerSet RunProc(string proc, int pageIndex, int pageSize, Dictionary<string, object> conditions)
        {
            var prams = new List<DbParameter>();
            prams.Add(Database.MakeInParam("pageIndex", pageIndex));
            prams.Add(Database.MakeInParam("pageSize", pageSize));
            foreach (var item in conditions)
            {
                prams.Add(Database.MakeInParam(item.Key, item.Value));
            }

            DataSet ds;
            Database.RunProc(proc, prams, out ds);
            int totalCount = 0;
            if (ds.Tables[0].Rows.Count > 0)
            {
                totalCount = Convert.ToInt32(ds.Tables[0].Rows[0]["totalCount"]);
            }
            int totalPage = (int)Math.Ceiling((double)totalCount / pageSize);
            PagerSet pageSet = new PagerSet(pageIndex, pageSize, totalPage, totalCount, ds);
            return pageSet;
        }

    }
}

