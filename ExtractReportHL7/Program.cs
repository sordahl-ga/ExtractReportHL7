/* 
* 2018 Microsoft Corp
* 
* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS “AS IS”
* AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
* THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
* ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
* FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
* HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
* OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
* OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ExtractReportHL7
{
    class Program
    {
        static void Main(string[] args)
        {
            //First argument is path to a file that contains a valid HL7 message.  (Must follow HL7 rules!!)
            if (args.Length > 0 && File.Exists(args[0])) {
                string hl7 = File.ReadAllText(args[0]);
                JObject obj = HL7ToXmlConverter.ConvertToJObject(hl7);
                string msgtype = obj["hl7message"]["MSH"]["MSH.9"].GetFirstField();
                StringBuilder builder = new StringBuilder();
                if (msgtype.Equals("ORU") || msgtype.Equals("MDM"))
                {
                    if (obj["hl7message"]["OBX"] is JArray)
                    {
                        foreach (var obx in obj["hl7message"]["OBX"])
                        {
                            if (Utilities.getFirstField(obx["OBX.2"]).Equals("TX") || Utilities.getFirstField(obx["OBX.2"]).Equals("FT"))
                            {
                                builder.Append(Utilities.getFirstField(obx["OBX.5"]).UnEscapeHL7());
                                builder.Append("\r\n");
                            }
                        }
                    }
                    else
                    {
                        if (Utilities.getFirstField(obj["hl7message"]["OBX"]["OBX.2"]).Equals("TX") || Utilities.getFirstField(obj["hl7message"]["OBX"]["OBX.2"]).Equals("FT"))
                        {
                            builder.Append(Utilities.getFirstField(obj["hl7message"]["OBX"]["OBX.5"]).UnEscapeHL7());
                            builder.Append("\r\n");
                        }
                    }
                    Console.WriteLine(builder.ToString());
                    Console.ReadLine();

                }
            } else
            {
                Console.WriteLine("Please pass in a valid path and file");
            }
        }
    }
}
