/**
 * Project: learnwsclient
 * 
 * @file    LearnCourses.cs
 * @author  Allen Vanderlinde, Copyright (C) 2016
 * @date    October 1, 2016
 * @brief   This class represents Learn courses and is used to
 *          expose various course data in the system (e.g., availability,
 *          batch UID, etc.).
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
     * @brief   This class represents Learn courses and is used to
     *          expose various course data in the system (e.g., availability,
     *          batch UID, etc.).
     */
    public class LearnCourses : Entity {
        /** @brief  Learn's virtual object array which stores individual course information. */
        private CourseVO[]                                              courses;
        /** @brief  Learn's course filter object used to tell the services what kind of data to pull. */
        private CourseFilter                                            filter;

        /**
         * @brief   Constructs a courses object.
         */
        public LearnCourses(COURSE_FILTER_TYPE _cft,
                            out bool success) {
            this.initDataSources();
            this.data = new List<string>();
            this.filter = new CourseFilter();
            this.filter.filterType = (int)_cft;
            this.filter.filterTypeSpecified = true;

            /* Initialize necessary members and objects
                before attempting to pull data. */
            switch(_cft) {
                case COURSE_FILTER_TYPE.GET_ALL_COURSES:
                    break;
                case COURSE_FILTER_TYPE.GET_COURSE_BY_COURSEID:
                    if(DataRequest.usingInputFile()) {
                        try {
                            this.filter.courseIds = System.IO.File.ReadAllLines(@DataRequest.getEntity());
                        } catch(System.IO.FileNotFoundException e) {
                            WebServices.setEventInfo(String.Format("\n\n FAIL: Unable to open \"{0}\" or it doesn't exist!\n\tMore info: {1}", DataRequest.getEntity(), e.Message));

                            success = false;

                            return;
                        }
                    } else {
                        this.filter.courseIds = new string[] { DataRequest.getEntity() };
                    }

                    break;
            };

            try {
                this.courses = WebServices.getCourseWS().loadCourses(this.filter);
                if(this.courses == null) {
                    throw new NullReferenceException();
                }
            } catch(Exception e) {
                success = false;
                WebServices.setEventInfo(String.Format("\n\n FAIL: Unable to build courses object with requested data!\n\tMore info: {0}\n\n\t*The specified course(s) most likely don't exist.", e.Message));

                return;
            }

            this.count = this.courses.Length;

            /* Populate entity's data list
                with pulled course data. */
            this.data.Add(this.getHeaders());
            for(int i = 0; i < this.count; i++) {
                if(this.courses[i].available) { this.availability = "Y"; } else { this.availability = "N"; }

                switch(DataRequest.getExportFormat()) {
                    default:    // Equivalent output to EXPORT_FORMAT.FLATFILE
                        this.record = String.Format("{1}{0}{2}{0}{3}{0}{5}{0}{6}",
                                        DataRequest.getDelimeter(),
                                        this.courses[i].batchUid,
                                        this.courses[i].courseId,
                                        this.courses[i].name,
                                        "",//"enabled",
                                        this.availability,
                                        this.dsk[this.courses[i].dataSourceId]);

                        break;
                    case EXPORT_FORMAT.CSV:
                        this.record = String.Format("\"{1}\"{0}\"{2}\"{0}\"{3}\"{0}\"{5}\"{0}\"{6}\"",
                                             DataRequest.getDelimeter(),
                                             this.courses[i].batchUid,
                                             this.courses[i].courseId,
                                             this.courses[i].name,
                                             "",//"enabled",
                                             this.availability,
                                             this.dsk[this.courses[i].dataSourceId]);

                        break;
                };                

                this.data.Add(this.record);
            }

            WebServices.setEventInfo(String.Format("\n SUCCESS: Course data request successful."));

            success = true;
        }

        /**
         * @brief   Return the PK1 (ID) string of a record at the specified
         *          index.
         * @retval  string  The string value of the PK1 (ID) of the entity
         *                  at the specified index of this object's CourseVO
         *                  array.
         */
        public override string getPK1(int index) { return this.courses[index].id; }

        /**
         * @brief   Return the batch UID string of a record at the specified
         *          index.
         * @retval  string  The string value of the batch UID of the entity
         *                  at the specified index of this object's CourseVO
         *                  array.
         */
        public override string getBatchUid(int index) { return this.courses[index].batchUid; }

        /**
         * @brief   Return a string of Learn course SIS headers.
         * @retval  string  SIS headers string.
         */
        public string getHeaders() {
            return String.Format("EXTERNAL_COURSE_KEY{0}COURSE_ID{0}COURSE_NAME{0}AVAILABLE_IND{0}DATA_SOURCE_KEY", DataRequest.getDelimeter());
        }

        /**
         * @brief   Returns whether course data was pulled
         *          after the request.
         * @retval  bool    True if there is at least 1
         *                  entry in the CourseVO array.
         */
        public override bool exists() { return (this.courses != null) ? true : false; }
    }
}
