/**
 * Project: learnwsclient
 * 
 * @file    ExportFile.cs
 * @author  Allen Vanderlinde, 2016 (refer to LICENSE.txt for license details)
 * @date    September 19, 2016
 * @brief   This class handles exporting requested data
 *          to file.
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
     * @brief   This class handles exporting requested data to
     *          file.
     */
    public class ExportFile {
        /** @brief  Path to write requested data to. */
        private string                                                  pathToFile;

        /**
         * @brief   Create a new file to write the requested data
         *          into.
         */
        public ExportFile(string            _ptf,
                          List<string>      data) {
            this.pathToFile = _ptf;

            try {
                WebServices.setEventInfo(String.Format("\n Writing requested data to file \"{0}\"...", this.pathToFile));
                WebServices.postEventInfo();
                System.IO.File.WriteAllLines(this.@pathToFile, data);
            } catch(Exception e) {
                WebServices.setEventInfo(String.Format("\n FAIL: Unable to write requested data to file!\n\tMore info: {0}", e.Message));
                WebServices.postEventInfo();

                return;
            }

            WebServices.setEventInfo(String.Format(" Wrote {0} records to file.", (data.Count - 1)));
            WebServices.postEventInfo();
        }

        /**
         * @brief   Return this file's particular path.
         */
        public string getPath() { return this.pathToFile; }
    }
}