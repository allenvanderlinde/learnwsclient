////////////////////////////////////////
LEARNWSCLIENT
Version: 1.0.1.0

Written by Allen Vanderlinde, 2016

Learnwsclient is a .NET command-line tool used to pull user, course, and enrollment data from a Blackboard Learn environment using Blackboard's SOAP web services. Learnwsclient is currently single-threaded but supports batch requests by pulling data on multiple entries stored in a file. Requested data can be exported to file by specifying a file name. Default export format is Snapshot FlatFile but data can be exported to a comma-separated values file as well.


////////////////////////////////////////
REGISTRATION and USAGE

Learnwsclient has a single option to register a proxy tool with the following syntax:

learnwsclient register [HOST] [TOOL_NAME] [TOOL_VENDOR] [TOOL_DESCRIPTION] [INITIAL_SECRET] [SHARED_SECRET]

       [HOST]                  The fully-qualified URL of the Learn environment to register to the tool in
       [TOOL_NAME]             The chosen name to register the proxy tool as
       [TOOL_VENDOR]           The chosen vendor of the tool
       [TOOL_DESCRIPTION]      A brief description of the proxy tool. Must be enclosed in ""
       [INITIAL_SECRET]        This is the required registration password configured in
                               Building Blocks > Proxy Tools > Manage Global Properties
       [SHARED_SECRET]         This is the password the registered tool will use to connect

       Example:
       learnwsclient register https://sub.client.edu learnwsclient bb "WS client." globalpassword toolpassword


To pull data from an environment with learnwsclient:

learnwsclient [HOST] [TOOL_NAME] [TOOL_VENDOR] [SHARED_SECRET] [ENTITY_OPERATION] [ENTITY] {EXPORT_FILE}

       Note: {EXPORT_FILE} is an optional argument. Currently supported file types are .CSV and .TXT

       Example:
       learnwsclient https://sub.client.edu learnwsclient bb toolpassword 7 bbsupport

       - This pulls all user enrollments for user "bbsupport"


The following entity operations are available:

       1       Request general user data for individual user entity.
       2       Request general users data for all user entities specified in a file.
       3       Request all users and their information on the system.
       4       Request general course data for an individual course entity.
       5       Request general courses data for all course entities specified in a file.
       6       Request all courses and their information on the system.
       7       Request course enrollments for a user by user ID.
       8       Request all course enrollments for all user entities specified in a file.
       9       Request enrollments for an individual course entity.
       10      Request enrollments for all course entities specified in a file.

*Note: A learnwsclient "entity" is the name, ID, or file of items which the client is pulling data on.


////////////////////////////////////////
NOTES ON USAGE:

1. Inputs are case-sensitive. BBTEST_COURSE1 is unique compared to bbtest_course1.

2. If you're using inputs from a file exported into the .csv format from a previous request make sure that Excel or your text editor doesn't format user or course IDs in a special way (e.g., Excel using scientific notation for especially long numerical course IDs, or removing leading 0s from numerical-only course IDs). Learnwsclient won't correct for this!

3. If you're using inputs from a file with misspelled course or user IDs, that particular entry will be skipped during the request without notification.


////////////////////////////////////////
DISCLAIMER

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*Refer to LICENSE.txt for more open-source/free software details

- BbWsClient.dll was built using Blackboard, Inc.'s "Blackboard Learn" web services libraries and is accompanied by its own license and requirements found in README_BBWSCLIENT.DLL.txt.

- Learnwsclient "cloud icon" used under GNU GPL version 3 (https://www.gnu.org/copyleft/gpl.html).