/**
 * Project: learnwsclient
 * 
 * @file    LearnUsers.cs
 * @author  Allen Vanderlinde, Copyright (C) 2016
 * @date    September 19, 2016
 * @brief   This class represents Learn users and is used to
 *          expose various user data in the system (e.g., name,
 *          student ID, batch UID, etc.).
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BbWsClient;

namespace learnwsclient
{
    /**
     * @brief   This class represents Learn users and is used to
     *          expose various user data in the system (e.g., name,
     *          student ID, batch UID, etc.).
     */
    public class LearnUsers : Entity {
        /** @brief  Learn's virtual object array which stores individual user information. */
        private UserVO[]                                                users;
        /** @brief  Learn's user filter object used to tell the services what kind of data to pull. */
        private UserFilter                                              filter;

        /**
         * @brief   Constructs a users object.
         */
        public LearnUsers(USER_FILTER_TYPE _uft,
                          out bool success) {
            this.initDataSources();
            this.data = new List<string>();
            this.filter = new UserFilter();
            this.filter.filterType = (int)_uft;
            this.filter.filterTypeSpecified = true;

            /* Initialize necessary members and objects
                before attempting to pull data. */
            switch(_uft) {
                case USER_FILTER_TYPE.GET_ALL_USERS_WITH_AVAILABILITY:                    
                    break;
                case USER_FILTER_TYPE.GET_USER_BY_NAME_WITH_AVAILABILITY:
                    if(DataRequest.usingInputFile()) {
                        try {
                            this.filter.name = System.IO.File.ReadAllLines(@DataRequest.getEntity());
                        } catch(System.IO.FileNotFoundException e) {
                            WebServices.setEventInfo(String.Format("\n\n FAIL: Unable to open \"{0}\" or it doesn't exist!\n\tMore info: {1}", DataRequest.getEntity(), e.Message));

                            success = false;

                            return;
                        }
                    } else {
                        this.filter.name = new string[] { DataRequest.getEntity() };
                    }

                    break;
            };

            try {
                this.users = WebServices.getUserWS().getUser(this.filter);
                if(this.users == null) {
                    throw new NullReferenceException();
                }
            } catch(Exception e) {
                WebServices.setEventInfo(String.Format("\n\n FAIL: Unable to build users object with requested data!\n\tMore info: {0}\n\n\t*The specified user(s) most likely don't exist.", e.Message));

                success = false;

                return;
            }

            this.count = this.users.Length;

            /* Populate entity's data list
                with pulled user data. */
            this.data.Add(this.getHeaders());
            for(int i = 0; i < this.count; i++) {
                if(this.users[i].isAvailable) { this.availability = "Y"; } else { this.availability = "N"; }

                switch(DataRequest.getExportFormat()) {
                    default:    // Equivalent output to EXPORT_FORMAT.FLATFILE
                        this.record = String.Format("{1}{0}{2}{0}{3}{0}{4}{0}{6}{0}{7}",
                                             DataRequest.getDelimeter(),
                                             this.users[i].userBatchUid,
                                             this.users[i].name,
                                             this.users[i].insRoles[0],
                                             this.users[i].systemRoles[0],
                                             "",//"enabled",
                                             this.availability,
                                             this.dsk[this.users[i].dataSourceId]);

                        break;
                    case EXPORT_FORMAT.CSV:
                        this.record = String.Format("\"{1}\"{0}\"{2}\"{0}\"{3}\"{0}\"{4}\"{0}\"{6}\"{0}\"{7}\"",
                                             DataRequest.getDelimeter(),
                                             this.users[i].userBatchUid,
                                             this.users[i].name,
                                             this.users[i].insRoles[0],
                                             this.users[i].systemRoles[0],
                                             "",//"enabled",
                                             this.availability,
                                             this.dsk[this.users[i].dataSourceId]);

                        break;
                };

                this.data.Add(this.record);
            }

            WebServices.setEventInfo(String.Format("\n SUCCESS: User data request successful."));

            success = true;
        }

        /**
         * @brief   Return the PK1 (ID) string of a record at the specified
         *          index.
         * @retval  string  The string value of the PK1 (ID) of the entity
         *                  at the specified index of this object's UserVO
         *                  array.
         */
        public override string getPK1(int index) { return this.users[index].id; }

        /**
         * @brief   Return the batch UID string of a record at the specified
         *          index.
         * @retval  string  The string value of the batch UID of the entity
         *                  at the specified index of this object's UserVO
         *                  array.
         */
        public override string getBatchUid(int index) { return this.users[index].userBatchUid; }

        /**
         * @brief   Return a string of Learn user SIS headers.
         * @retval  string  SIS headers string.
         */
        public string getHeaders() {
            return String.Format("EXTERNAL_PERSON_KEY{0}USER_ID{0}INSTITUTION_ROLE{0}SYSTEM_ROLE{0}AVAILABLE_IND{0}DATA_SOURCE_KEY", DataRequest.getDelimeter());
        }

        /**
         * @brief   Returns whether user data was pulled
         *          after the request.
         * @retval  bool    True if there is at least 1
         *                  entry in the UserVO array.
         */
        public override bool exists() { return (this.users != null) ? true : false; }
    }
}