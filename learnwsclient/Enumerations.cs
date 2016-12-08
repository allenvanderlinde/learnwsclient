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

using BbWsClient;

namespace learnwsclient
{
    /** @defgroup   Enumerations    Enumerations
     *  Command line, object filter, and file type constants.
     * @{
     */
    
    /**
     * @brief   Command line login type options.
     */
    public enum LOGIN_TYPE
    {
        LOGIN_AS_USER                       = 1,    /**< Client will log in as a user. */
        LOGIN_AS_TOOL                       = 2     /**< Client will log in as a proxy tool. */
    };

    /**
     * @brief   A basic list of operations to perform by
     *          the client.
     */
    public enum ENTITY_OPERATION
    {
        USER_INFO_BY_USERID                               = 1,      /**< Request general user data for individual user entity. */
        USERS_INFO_BY_USERIDS_FROM_FILE                   = 2,      /**< Request general users data for all user entities specified in a file. */
        USERS_INFO_ALL                                    = 3,      /**< Request all users and their information on the system. */
        COURSE_INFO_BY_COURSEID                           = 4,      /**< Request general course data for an individual course entity. */
        COURSES_INFO_BY_COURSEIDS_FROM_FILE               = 5,      /**< Request general courses data for all course entities specified in a file. */
        COURSES_INFO_ALL                                  = 6,      /**< Request all courses and their information on the system. */
        USER_ENROLLMENT_INFO_BY_USERID                    = 7,      /**< Request course enrollments for a user by user ID. */
        USERS_ENROLLMENT_INFO_BY_USERIDS_FROM_FILE        = 8,      /**< Request all course enrollments for all user entities specified in a file. */
        COURSE_ENROLLMENT_INFO_BY_COURSEID                = 9,      /**< Request enrollments for an individual course entity. */
        COURSES_ENROLLMENT_INFO_BY_COURSEIDS_FROM_FILE    = 10,     /**< Request enrollments for all course entities specified in a file. */
        USER_MODIFY_ROW_STATUS                            = 11      /**< Request changing the row status of a user record. */
        //USER_ENROLLMENT_INFO_BY_USERID                    = 7,      /**< Request enrollments for an individual user. */
        //USERS_ENROLLMENT_INFO_BY_FILE_BY_USERIDS          = 8,      /**< Request enrollments for all user entities specified in a file. */
        //COURSE_ENROLLMENT_INFO_BY_COURSEIDS_FROM_FILE     = 10      /**< Request enrollments for all course entities specified in a file. */
    };

    /**
     * @brief   Filter types used to define the kind of
     *          user data desired. Used as a wrapper for
     *          Learn's UserWSConstants.
     *          See: /User.WS/blackboard/ws/user/UserWSConstants.html
     */
    public enum USER_FILTER_TYPE
    {
        GET_ALL_USERS_WITH_AVAILABILITY             = 1,    /**< Pull all users. */
        GET_USER_BY_ID_WITH_AVAILABILITY            = 2,    /**< Pull user data by PK1. */
        GET_USER_BY_NAME_WITH_AVAILABILITY          = 6,    /**< Pull user data by user ID. */
    };

    /**
     * @brief   Filter types used to define the kind of
     *          course data desired. Used as a wrapper for
     *          Learn's CourseWSConstants.
     *          See: /Course.WS/blackboard/ws/course/CourseWSConstants.html
     */
    public enum COURSE_FILTER_TYPE
    {
        GET_ALL_COURSES                             = 0,    /**< Pull all courses. */
        GET_COURSE_BY_COURSEID                      = 1,    /**< Pull course data by course ID. */
        GET_COURSE_BY_ID                            = 3     /**< Pull course data by PK1. */
    };

    /**
     * @brief   Filter types used to define the kind of
     *          enrollment data desired. Used as a wrapper for
     *          Learn's CourseMembershipWSConstants.
     *          See: /CourseMembership.WS/blackboard/ws/coursemembership/CourseMembershipWSConstants.html
     */
    public enum ENROLLMENT_FILTER_TYPE
    {
        GET_ENROLLMENTS_BY_COURSE_ID                = 2,    /**< Pull enrolled users list by course ID. */
        GET_ENROLLMENTS_BY_USER_ID                  = 5     /**< Pull courses list of user by user ID. */
    };

    /**
     * @brief   Export file formats.
     */
    public enum EXPORT_FORMAT
    {
        FLATFILE            = 1,    /**< Snapshot Flat File. */
        CSV                 = 2     /**< Comma-separated values file. */
    };

    /** @} */
}
