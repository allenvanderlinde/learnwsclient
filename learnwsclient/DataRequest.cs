/**
 * Project: learnwsclient
 * 
 * @file    DataRequest.cs
 * @author  Allen Vanderlinde, Copyright (C) 2016
 * @date    September 19, 2016
 * @brief   This class handles the actual data request chosen
 *          by the user.
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
     * @brief   This class handles the actual data request
     *          chosen by the user.
     */
    public class DataRequest {
        /** @brief  Arguments list structure defining what to do for the session. */
        private static Arguments                                        args;
        /** @brief  Store if the request was successfully sent and results received. */
        private bool                                                    successful;

        /** @brief  Type of operation to perform on the desired Learn object (entity). */
        private static ENTITY_OPERATION                                 entityOperation;
        /** @brief  Type of Learn object to pull data from (e.g., users, courses). */
        private static string                                           entity;

        /** @brief  Client's users object to build from request. */
        private LearnUsers                                              users;
        /** @brief  Learn filter for users. */
        private USER_FILTER_TYPE                                        uft;

        /** @brief  Client's courses object to build from request. */
        private LearnCourses                                            courses;
        /** @brief  Learn filter for courses. */
        private COURSE_FILTER_TYPE                                      cft;

        /** @brief  Client's enrollments object to build from request. */
        private LearnEnrollments                                        enrollments;
        /** @brief  Learn filter for enrollments (course memberships). */
        private ENROLLMENT_FILTER_TYPE                                  eft;

        /** @brief  Export file format to write pulled data to after request. */
        private static EXPORT_FORMAT                                    exportFormat;
        /** @brief  Delimeter to use between data when exporting to file. */
        private static char                                             delim                   = '|';
        
        /**
         * @brief   Here we can pass in the relevant arguments from the
         *          command line to tell the client what sort of data
         *          we're interested in. The class then builds the necessary
         *          Learn objects whose results are written to the console
         *          or file.
         */
        public DataRequest(Arguments _args) {
            //args = _args;

            switch(DataRequest.getEntityOperation()) {
                case ENTITY_OPERATION.USER_INFO_BY_USERID:
                    this.uft = USER_FILTER_TYPE.GET_USER_BY_NAME_WITH_AVAILABILITY;
                    this.users = new LearnUsers(this.uft, out successful);

                    break;
                case ENTITY_OPERATION.USERS_INFO_BY_USERIDS_FROM_FILE:
                    this.uft = USER_FILTER_TYPE.GET_USER_BY_NAME_WITH_AVAILABILITY;
                    this.users = new LearnUsers(this.uft, out successful);

                    break;
                case ENTITY_OPERATION.USERS_INFO_ALL:
                    this.uft = USER_FILTER_TYPE.GET_ALL_USERS_WITH_AVAILABILITY;
                    this.users = new LearnUsers(this.uft, out successful);

                    break;
                case ENTITY_OPERATION.COURSE_INFO_BY_COURSEID:
                    this.cft = COURSE_FILTER_TYPE.GET_COURSE_BY_COURSEID;
                    this.courses = new LearnCourses(this.cft, out successful);

                    break;
                case ENTITY_OPERATION.COURSES_INFO_BY_COURSEIDS_FROM_FILE:
                    this.cft = COURSE_FILTER_TYPE.GET_COURSE_BY_COURSEID;
                    this.courses = new LearnCourses(this.cft, out successful);

                    break;
                case ENTITY_OPERATION.COURSES_INFO_ALL:
                    this.cft = COURSE_FILTER_TYPE.GET_ALL_COURSES;
                    this.courses = new LearnCourses(this.cft, out successful);

                    break;
                case ENTITY_OPERATION.USER_ENROLLMENT_INFO_BY_USERID:
                    this.eft = ENROLLMENT_FILTER_TYPE.GET_ENROLLMENTS_BY_USER_ID;
                    this.enrollments = new LearnEnrollments(this.eft, out successful);

                    break;
                case ENTITY_OPERATION.USERS_ENROLLMENT_INFO_BY_USERIDS_FROM_FILE:
                    this.eft = ENROLLMENT_FILTER_TYPE.GET_ENROLLMENTS_BY_USER_ID;
                    this.enrollments = new LearnEnrollments(this.eft, out successful);

                    break;
                case ENTITY_OPERATION.COURSE_ENROLLMENT_INFO_BY_COURSEID:
                    this.eft = ENROLLMENT_FILTER_TYPE.GET_ENROLLMENTS_BY_COURSE_ID;
                    this.enrollments = new LearnEnrollments(this.eft, out successful);

                    break;
                case ENTITY_OPERATION.COURSES_ENROLLMENT_INFO_BY_COURSEIDS_FROM_FILE:
                    this.eft = ENROLLMENT_FILTER_TYPE.GET_ENROLLMENTS_BY_COURSE_ID;
                    this.enrollments = new LearnEnrollments(this.eft, out successful);

                    break;
            };
        }

        /**
         * @brief   Exports requested data to file.
         */
        public void export() {
            ExportFile file;

            switch((ENTITY_OPERATION)Convert.ToInt32(args.entityOperation)) {
                case ENTITY_OPERATION.USER_INFO_BY_USERID:
                    file = new ExportFile(args.exportFile, this.users.getData());

                    break;
                case ENTITY_OPERATION.USERS_INFO_BY_USERIDS_FROM_FILE:
                    file = new ExportFile(args.exportFile, this.users.getData());

                    break;
                case ENTITY_OPERATION.USERS_INFO_ALL:
                    file = new ExportFile(args.exportFile, this.users.getData());

                    break;
                case ENTITY_OPERATION.COURSE_INFO_BY_COURSEID:
                    file = new ExportFile(args.exportFile, this.courses.getData());

                    break;
                case ENTITY_OPERATION.COURSES_INFO_BY_COURSEIDS_FROM_FILE:
                    file = new ExportFile(args.exportFile, this.courses.getData());

                    break;
                case ENTITY_OPERATION.COURSES_INFO_ALL:
                    file = new ExportFile(args.exportFile, this.courses.getData());

                    break;
                case ENTITY_OPERATION.USER_ENROLLMENT_INFO_BY_USERID:
                    file = new ExportFile(args.exportFile, this.enrollments.getData());

                    break;
                case ENTITY_OPERATION.USERS_ENROLLMENT_INFO_BY_USERIDS_FROM_FILE:
                    file = new ExportFile(args.exportFile, this.enrollments.getData());

                    break;
                case ENTITY_OPERATION.COURSE_ENROLLMENT_INFO_BY_COURSEID:
                    file = new ExportFile(args.exportFile, this.enrollments.getData());

                    break;
                case ENTITY_OPERATION.COURSES_ENROLLMENT_INFO_BY_COURSEIDS_FROM_FILE:
                    file = new ExportFile(args.exportFile, this.enrollments.getData());

                    break;
            };
        }

        /**
         * @brief   Set's the export format to write data to after request.
         * @param[in]   ext  The file extension to determine the export
         *                   format with.
         */
        public static void setExportFormat(string ext) {
            if(ext.ToLower() == "csv") {
                exportFormat = EXPORT_FORMAT.CSV;
                delim = ',';
            } else {
                exportFormat = EXPORT_FORMAT.FLATFILE;
                delim = '|';
            }
        }

        /**
         * @brief   Set the operation the client should
         *          perform on the data.
         * @param[in]   _eop    A member of the learnwsclient.ENTITY_OPERATION
         *                      enumeration specifying the kind of data to pull.
         */
        public static void setEntityOperation(ENTITY_OPERATION _eop) { entityOperation = _eop; }
        /**
         * @brief   Set the specific item to get the chosen kind of
         *          data from (e.g., a user ID, a course ID, a file
         *          with a list of courses, etc.).
         * @param[in]   _entity The specific item or thing to pull data from.
         */
        public static void setEntity(string _entity) { entity = _entity; }

        /**
         * @brief   Returns users object.
         * @retval  LearnUsers  Object with request's user data.
         */
        public LearnUsers getUsers() { return this.users; }

        /**
         * @brief   Returns courses object.
         * @retval  LearnCourses    Object with request's course data.
         */
        public LearnCourses getCourses() { return this.courses; }

        /**
         * @brief   Returns enrollments object.
         * @retval  LearnEnrollments    Object with request's enrollment data.
         */
        public LearnEnrollments getEnrollments() { return this.enrollments; }

        /**
         * @brief   Get the determined export format from the request.
         * @retval  EXPORT_FORMAT   The export format.
         */
        public static EXPORT_FORMAT getExportFormat() { return exportFormat; }

        /**
         * @brief   Get the kind of data chosen to be returned
         *          by the client.
         * @retval  ENTITY_OPERATION  A member of the learnwsclient.ENTITY_OPERATION
         *                            enumeration specifying the kind of data pulled.
         */
        public static ENTITY_OPERATION getEntityOperation() { return entityOperation; }
        /**
         * @brief   Get the item selected to get the chosen
         *          data from.
         * @retval  string  The specific item or thing data is being pulled from.
         */
        public static string getEntity() { return entity; }

        /**
         * @brief   Returns the delimeter character determined from
         *          the requested data's export format.
         * @retval  char    The delimeter character.
         */
        public static char getDelimeter() { return delim; }

        /**
         * @brief   Returns whether the request should be using inputs
         *          from a file.
         * @retval  bool    True if a file is being used with inputs to pull data from.
         */
        public static bool usingInputFile() { return args.useInputFile; }

        /**
         * @brief   Returns success or failure of request.
         * @retval  bool    True if request sent successfully.
         */
        public bool isSuccessful() { return this.successful; }
    }
}