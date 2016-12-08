/**
 * Project: learnwsclient
 * 
 * @file    LearnEnrollments.cs
 * @author  Allen Vanderlinde, 2016 (refer to LICENSE.txt for license details)
 * @date    October 6, 2016
 * @brief   This class represents Learn enrollments and is used to
 *          expose various user membership data in the system.
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
     * @brief   This class represents Learn enrollments and is used to
     *          expose various user membership data in the system.
     */
    public class LearnEnrollments : Entity {

        /** @brief  CourseMembershipVO array List used to store enrollment records for users or courses. */
        private List<CourseMembershipVO[]>                              enrollments_matrix;

        /** @brief  Learn's course membership filter object used to tell the services what kind of data to pull. */
        private MembershipFilter                                        filter;
        /** @brief  Enrollment's filter type enumeration used for convenience in this class. */
        private ENROLLMENT_FILTER_TYPE                                  eft;

        /** @brief  UserVO object array used to complete enrollment data request when identifying enrolled users. */
        private UserVO[]                                                users;
        /** @brief  Learn's user filter object used to complete exposition of enrolled user data. */
        private UserFilter                                              uf;

        /** @brief  CourseVO object array used to complete enrollment data request when identifying courses
         *          user(s) are enrolled in. */
        private CourseVO[]                                              courses;
        /** @brief  Learn's course filter object used to complete exposition of course enrollments for users. */
        private CourseFilter                                            cf;

        /** @brief  Determines the top-level nested loop constraint when populating entity's data list with inputs from file. */
        private int                                                     dataListMax;

        /** @brief  List which holds users with no enrollments used to exclude users from enrollments data list. */
        private List<string>                                            usersWithEnrollments;
        /** @brief  List which holds courses with at list 1 enrollment when building the enrollments data list. */
        private List<string>                                            nonEmptyCourses;

        /** @brief  Counter used as an index to grab user or course data from when writing an individual enrollment record.
                    This integer increments linerally to match the index of the CourseVO or UserVO object arrays when assigning
                    course IDs or user IDs for enrollments when populating the data list.*/
        private static int                                              counter;

        /**
         * @brief   Constructs an enrollments object.
         */
        public LearnEnrollments(ENROLLMENT_FILTER_TYPE _eft,
                                out bool success) {
            this.eft = _eft;
            this.enrollments_matrix = new List<CourseMembershipVO[]>();
            this.initDataSources();
            this.data = new List<string>();
            this.filter = new MembershipFilter();
            this.filter.filterType = (int)this.eft;
            this.filter.filterTypeSpecified = true;

            this.uf = new UserFilter();
            this.cf = new CourseFilter();

            this.dataListMax = 0;
            counter = 0;

            /* Initialize necessary members and objects
                before attempting to pull data. */
            switch (this.eft) {
                case ENROLLMENT_FILTER_TYPE.GET_ENROLLMENTS_BY_COURSE_ID:
                    /* Since we're working with course IDs (not PK1s) for the
                        user's sake, we must first build a CourseVO object array
                        based on the course ID specified in the CLI.

                        Then we populate a UserVO object array from the user PK1s
                        which populate the CourseMembershipVO array in order to
                        expose each course enrollment's user ID. */
                    try {
                        this.cf.filterType = (int)COURSE_FILTER_TYPE.GET_COURSE_BY_COURSEID;
                        this.cf.filterTypeSpecified = true;

                        if(DataRequest.usingInputFile()) {
                            try {
                                this.cf.courseIds = System.IO.File.ReadAllLines(@DataRequest.getEntity());
                            } catch(System.IO.FileNotFoundException e) {
                                WebServices.setEventInfo(String.Format("\n\n FAIL: Unable to open \"{0}\" or it doesn't exist!\n\tMore info: {1}", DataRequest.getEntity(), e.Message));

                                success = false;

                                return;
                            }
                        } else {
                            this.cf.courseIds = new string[] { DataRequest.getEntity() };
                        }

                        this.courses = WebServices.getCourseWS().loadCourses(this.cf);

                        if(this.courses == null) {
                            throw new NullReferenceException();
                        }
                    } catch(Exception e) {
                        WebServices.setEventInfo(String.Format("\n\n FAIL: Unable to load course information while building enrollments object!\n\tMore info: {0}\n\n\t*The course(s) most likely don't exist.", e.Message));

                        success = false;

                        return;
                    }

                    this.dataListMax = this.courses.Length;
                    if(!this.buildEnrollments()) {
                        success = false;

                        return;
                    }

                    /* Populate the UserVO array to expose
                        user IDs whose enrollments are in
                        the specified course(s). */
                    try {
                        string[] userPK1s = new string[this.count];

                        this.uf.filterType = (int)USER_FILTER_TYPE.GET_USER_BY_ID_WITH_AVAILABILITY;
                        this.uf.filterTypeSpecified = true;

                        for(int i = 0; i < this.dataListMax; i++) {
                            for(int j = 0; j < this.enrollments_matrix[i].Length; j++) {
                                userPK1s[counter] = this.enrollments_matrix[i][j].userId;
                                counter++;
                            }
                        }
                        counter = 0;    // Reset counter
                        this.uf.id = userPK1s;
                        this.users = WebServices.getUserWS().getUser(uf);

                        if(this.users == null) {
                            throw new NullReferenceException();
                        }
                    } catch(Exception e) {
                        WebServices.setEventInfo(String.Format("\n\n FAIL: Unable to load user information for enrollments request!\n\tMore info: {0}", e.Message));

                        success = false;

                        return;
                    }

                    break;
                case ENROLLMENT_FILTER_TYPE.GET_ENROLLMENTS_BY_USER_ID:
                    /* Since we're working with user IDs (not PK1s) for the
                        user's sake, we must first build a UserVO object array
                        based on the user ID specified in the CLI.

                        Then we populate a CourseVO object array from the course PK1s
                        which populate the CourseMembershipVO array in order to
                        expose each of the user's enrollment course ID. */
                    try {
                        this.uf.filterType = (int)USER_FILTER_TYPE.GET_USER_BY_NAME_WITH_AVAILABILITY;
                        this.uf.filterTypeSpecified = true;

                        if(DataRequest.usingInputFile()) {
                            try {
                                this.uf.name = System.IO.File.ReadAllLines(@DataRequest.getEntity());
                            } catch(System.IO.FileNotFoundException e) {
                                WebServices.setEventInfo(String.Format("\n\n FAIL: Unable to open \"{0}\" or it doesn't exist!\n\tMore info: {1}", DataRequest.getEntity(), e.Message));

                                success = false;

                                return;
                            }
                        } else {
                            this.uf.name = new string[] { DataRequest.getEntity() };
                        }

                        this.users = WebServices.getUserWS().getUser(this.uf);

                        if(DataRequest.usingInputFile()) {
                            this.filter.userIds = new string[this.users.Length];
                            for(int i = 0; i < this.users.Length; i++) {
                                this.filter.userIds[i] = this.users[i].id;
                            }
                        } else {
                            this.filter.userIds = new string[] { this.users[0].id };
                        }

                        if(this.users == null) {
                            throw new NullReferenceException();
                        }
                    } catch(Exception e) {
                        WebServices.setEventInfo(String.Format("\n\n FAIL: Unable to load user information while building enrollments object!\n\tMore info: {0}\n\n\t*The user(s) most likely don't exist.", e.Message));

                        success = false;

                        return;
                    }

                    this.dataListMax = 1;
                    if(!this.buildEnrollments()) {
                        success = false;

                        return;
                    }

                    /* Populate the CourseVO array to expose
                        the course IDs of the specified user(s)
                        enrollments. */
                    try {
                        string[] coursePK1s = new string[this.count];

                        this.cf.filterType = (int)COURSE_FILTER_TYPE.GET_COURSE_BY_ID;
                        this.cf.filterTypeSpecified = true;

                        for(int i = 0; i < this.dataListMax; i++) {
                            for(int j = 0; j < this.enrollments_matrix[i].Length; j++) {
                                coursePK1s[counter] = this.enrollments_matrix[i][j].courseId;
                                counter++;
                            }
                        }
                        counter = 0;
                        this.cf.ids = coursePK1s;
                        this.courses = WebServices.getCourseWS().loadCourses(this.cf);

                        if(this.courses == null) {
                            throw new NullReferenceException();
                        }
                    } catch(Exception e) {
                        WebServices.setEventInfo(String.Format("\n\n FAIL: Unable to load course information for enrollments request!\n\tMore info: {0}", e.Message));

                        success = false;

                        return;
                    }

                    break;
            };

            /* Populate entity's data list
                with pulled enrollment data. */
            string courseBatchUidForRecord = "";
            string userBatchUidForRecord = "";

            this.data.Add(this.getHeaders());
            for(int i = 0; i < this.dataListMax; i++) {
                for(int j = 0; j < this.enrollments_matrix[i].Length; j++) {
                    if((bool)this.enrollments_matrix[i][j].available) { this.availability = "Y"; } else { this.availability = "N"; }

                    /* Determine with batch UIDs to associate
                        to the record. */
                    if(this.eft == ENROLLMENT_FILTER_TYPE.GET_ENROLLMENTS_BY_COURSE_ID) {
                        courseBatchUidForRecord = this.nonEmptyCourses[i];
                        userBatchUidForRecord = this.users[counter].userBatchUid;
                    } else if(this.eft == ENROLLMENT_FILTER_TYPE.GET_ENROLLMENTS_BY_USER_ID) {
                        courseBatchUidForRecord = this.enrollments_matrix[i][j].courseId;
                        for(int k = 0; k < this.courses.Length; k++) {
                            if(this.enrollments_matrix[i][j].courseId.Equals(this.courses[k].id)) {
                                courseBatchUidForRecord = this.courses[k].batchUid;
                                if(DataRequest.getEntityOperation() == ENTITY_OPERATION.USER_ENROLLMENT_INFO_BY_USERID) {
                                    userBatchUidForRecord = this.usersWithEnrollments[0];
                                } else {
                                    userBatchUidForRecord = this.usersWithEnrollments[j];
                                }
                            }
                        }
                    }

                    switch(DataRequest.getExportFormat()) {
                        default:    // Equivalent output to EXPORT_FORMAT.FLATFILE
                            this.record = String.Format("{1}{0}{2}{0}{3}{0}{5}{0}{6}",
                                                 DataRequest.getDelimeter(),
                                                 courseBatchUidForRecord,
                                                 userBatchUidForRecord,
                                                 this.enrollments_matrix[i][j].roleId,
                                                 "",//"enabled",
                                                 this.availability,
                                                 this.dsk[this.enrollments_matrix[i][j].dataSourceId]);

                            break;
                        case EXPORT_FORMAT.CSV:
                            this.record = String.Format("\"{1}\"{0}\"{2}\"{0}\"{3}\"{0}\"{5}\"{0}\"{6}\"",
                                                 DataRequest.getDelimeter(),
                                                 courseBatchUidForRecord,
                                                 userBatchUidForRecord,
                                                 this.enrollments_matrix[i][j].roleId,
                                                 "",//"enabled",
                                                 this.availability,
                                                 this.dsk[this.enrollments_matrix[i][j].dataSourceId]);

                            break;
                    };

                    this.data.Add(this.record);
                    counter++;
                }
            }

            WebServices.setEventInfo(String.Format("\n SUCCESS: Enrollment data request successful."));

            success = true;
        }

        /**
         * @brief   Builds the enrollments object prior to refining user
         *          and course data.
         * @retval  bool    True if enrollments object built successfully.
         */
        private bool buildEnrollments() {
            int num_enrollments = 0;    // Aggregate count of enrollments
            /* Temp Learn enrollments object array
                reused when pulling enrollment data
                for users and courses. */
            CourseMembershipVO[] temp = null;

            /* Attempt to build enrollments
                object based upon whether course
                IDs or user IDs were used as arguments
                from the CLI or from a file. */
            switch (this.eft) {
                case ENROLLMENT_FILTER_TYPE.GET_ENROLLMENTS_BY_COURSE_ID:
                    this.nonEmptyCourses = new List<string>();
                    if (DataRequest.usingInputFile()) {
                        for (int i = 0; i < this.courses.Length; i++) {
                            try {
                                temp = WebServices.getCourseMembershipWS().loadCourseMembership(this.courses[i].id, this.filter);
                                if(temp == null) {
                                    throw new NullReferenceException();
                                }
                            } catch(NullReferenceException e) {
                                WebServices.setEventInfo(String.Format("\n WARNING: Course \"{0}\" has no enrollments.", this.courses[i].courseId));
                                WebServices.postEventInfo();

                                this.dataListMax--; // Reduce the nested loop constraint when building data list since a course had no enrollments
                            }
                            if(temp != null) {
                                this.enrollments_matrix.Add(temp);
                                this.nonEmptyCourses.Add(this.courses[i].batchUid);
                                num_enrollments += temp.Length;
                            }
                        }
                    } else {
                        try {
                            this.enrollments_matrix.Add(WebServices.getCourseMembershipWS().loadCourseMembership(this.courses[this.dataListMax - 1].id, this.filter));
                            this.nonEmptyCourses.Add(this.courses[this.dataListMax - 1].batchUid);
                            num_enrollments += this.enrollments_matrix[0].Length;
                        } catch(Exception e) {
                            WebServices.setEventInfo(String.Format("\n\n FAIL: Unable to build enrollments object with requested data!\n\tMore info: {0}\n\n\t*The course most likely has no enrollments.", e.Message));

                            return false;
                        }
                    }

                    break;
                case ENROLLMENT_FILTER_TYPE.GET_ENROLLMENTS_BY_USER_ID:
                    this.usersWithEnrollments = new List<string>();
                    if (DataRequest.usingInputFile()) {
                        try {
                            temp = WebServices.getCourseMembershipWS().loadCourseMembership(null, this.filter);
                            if(temp == null) {
                                throw new NullReferenceException();
                            }
                        } catch(NullReferenceException e) {
                            WebServices.setEventInfo(String.Format("\n\n FAIL: Unable to build enrollments object with requested data!\n\tMore info: {0} ", e.Message));
                            WebServices.postEventInfo();

                            return false;
                        }
                        if(temp != null) {
                            /* Determine list of users who
                                have at least 1 enrollment. */
                            for(int i = 0; i < temp.Length; i++) {
                                for(int j = 0; j < this.users.Length; j++) {
                                    if(temp[i].userId.Equals(this.users[j].id)) {
                                        this.usersWithEnrollments.Add(this.users[j].userBatchUid);

                                        break;
                                    }
                                }
                            }
                            this.enrollments_matrix.Add(temp);
                            num_enrollments += temp.Length;

                            /* Report users who have
                                no enrollments. */
                            bool noEnrollment = true;
                            for(int i = 0; i < this.users.Length; i++) {
                                for(int j = 0; j < this.usersWithEnrollments.Count(); j++) {
                                    if(this.users[i].userBatchUid.Equals(this.usersWithEnrollments[j])) {
                                        noEnrollment = false;

                                        break;
                                    }
                                }
                                if(noEnrollment) {
                                    WebServices.setEventInfo(String.Format("\n WARNING: User \"{0}\" has no course enrollments.", this.users[i].name));
                                    WebServices.postEventInfo();
                                }
                                noEnrollment = true;
                            }
                        }
                    } else {
                        try {
                            this.enrollments_matrix.Add(WebServices.getCourseMembershipWS().loadCourseMembership(null, this.filter));
                            num_enrollments += this.enrollments_matrix[0].Length;
                        } catch(Exception e) {
                            WebServices.setEventInfo(String.Format("\n\n FAIL: Unable to build enrollments object with requested data!\n\tMore info: {0}\n\n*The user most likely has no enrollments.", e.Message));

                            return false;
                        }

                        this.usersWithEnrollments.Add(this.users[0].userBatchUid);
                    }

                    break;
            };

            this.count = num_enrollments;

            return true;
        }

        /**
         * @brief   Return a string of Learn enrollment SIS headers.
         * @retval  string  SIS headers string.
         */
        public string getHeaders() {
            return String.Format("EXTERNAL_COURSE_KEY{0}EXTERNAL_PERSON_KEY{0}ROLE{0}AVAILABLE_IND{0}DATA_SOURCE_KEY", DataRequest.getDelimeter());
        }

        /**
         * @brief   Returns whether enrollment data was pulled
         *          after the request.
         * @retval  bool    True if there is at least 1
         *                  entry in the CourseMembershipVO
         *                  array.
         */
        public override bool exists() { return (this.enrollments_matrix != null) ? true : false; }
    }
}
