/**
 * Project: learnwsclient
 * 
 * @file    Entity.cs
 * @author  Allen Vanderlinde, 2016 (refer to LICENSE.txt for license details)
 * @date    September 25, 2016
 * @brief   This is the base class representing
 *          users, courses, or other Learn
 *          objects.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace learnwsclient
{
    /**
     * @brief   This is the base class representing users, courses,
     *          or other Learn objects.
     */
    public class Entity {
        /** @brief  Data sources object array. */
        protected DataSourceVO[]                                        datasources;
        /** @brief  Dictionary of data source batch UIDs and their IDs. */
        protected Dictionary<string, string>                            dsk;
        /** @brief  String list which stores pulled data. */
        protected List<string>                                          data;
        /** @brief  Length of this entity's Learn object array. */
        protected int                                                   count;
        /** @brief  Single record data for Learn object item built from the specified entity. Printed in the console or exported to file. */
        protected string                                                record                  = "";
        /** @brief  String value for the availability of an entity's record.
                    Overwritten as needed per record during request's data pull. */
        protected string                                                availability;

        /**
         * @brief   Initialize data sources object array and dictionary.
         */
        protected void initDataSources() {
            this.datasources = WebServices.getUtilWS().getDataSources();
            this.dsk = new Dictionary<string, string>();

            for(int i = 0; i < this.getDataSourcesCount(); i++) {
                this.dsk.Add(datasources[i].id, datasources[i].batchUid);
            }
        }

        /**
         * @brief   Returns the number of data sources on the system.
         */
        protected int getDataSourcesCount() { return this.datasources.Length; }

        /**
         * @brief   Return requested data as a string
         *          list.
         */
        public List<string> getData() { return this.data; }

        /**
         * @brief   Return the PK1 (ID) string of a record at the specified
         *          index.
         */
        public virtual string getPK1(int index) { return ""; }

        /**
         * @brief   Return the batch UID string of a record at the specified
         *          index.
         */
        public virtual string getBatchUid(int index) { return ""; }

        /**
         * @brief   Returns the number of Learn entities
         *          stored in the derived object.
         */
        public virtual int getCount() { return this.count; }

        /**
         * @brief   Returns whether an instance
         *          of this object is not null
         *          going by the state of its
         *          Learn object.
         */
        public virtual bool exists() { return true; }
    }
}
