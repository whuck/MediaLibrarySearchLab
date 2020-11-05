using System;
using System.IO;
using NLog.Web;
using System.Collections.Generic;

namespace MediaLibrary
{
    public class MovieFile
    {
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        public List<Movie> MovieList;
        public MovieFile(string scrubbedFileName) {
            logger.Info("creating movieList");
            this.MovieList = ParseFile(scrubbedFileName);
            logger.Info("scrubbed file parsed into List<Movie>");
        }
        public void DisplayMovies() {
            foreach (Movie m in MovieList) {
                Console.WriteLine(m.Display());
            }
        }
        public ulong GetNewID(){
            ulong id = 0;
            foreach (Movie m in this.MovieList) {
                id = m.mediaId > id ? m.mediaId : id;
            }
            return id+1;
        }
        private List<Movie> ParseFile(string fileName) {
            logger.Info($"parsing file {fileName}.");

            List<Movie> outputList = new List<Movie>();
            if(System.IO.File.Exists(fileName)) {
                StreamReader sr = new StreamReader(fileName);
                while(!sr.EndOfStream) {
                    //11,"American President, The (1995)",Comedy|Drama|Romance,unassigned,00:00:00
                    string line = sr.ReadLine();
                    // grab 3rd last comma, substring will be everything after title
                    int idx = line.LastIndexOf(",");
                    idx = line.LastIndexOf(",",idx-1);
                    idx = line.LastIndexOf(",",idx-1);

                    string genreDirectorRuntime = line.Substring(idx+1);

                    //grab id & title
                    string idTitle = line.Substring( 0 , (line.Length - genreDirectorRuntime.Length - 1));
                    //logger.Info($"{line}");
                    //logger.Info($"{idTitle}+{genreDirectorRuntime}asdfsadf{idTitle.Substring(0,(line.IndexOf(",")))}");

                    string id = idTitle.Substring(0,(line.IndexOf(",")));
                    UInt64 m_id = UInt64.Parse(id);

                    string m_title = idTitle.Substring(line.IndexOf(",")+1);

                    string[] movieProps = genreDirectorRuntime.Split(",");
                    Movie m = new Movie {
                        mediaId =  m_id,
                        title = m_title,
                        //genres = movieProps[2].Split("|"),
                        director = movieProps[1],
                        runningTime = TimeSpan.Parse(movieProps[2])
                    };
                    //logger.Info($"movie{m.mediaId} object created!");
                    List<string> genres = new List<string>();
                    foreach(string genre in movieProps[0].Split("|")) {
                        genres.Add(genre);
                    }
                    m.genres = genres;
                    outputList.Add(m);
                    //logger.Info(m.Display());
                }
                sr.Close();
                return outputList;
            } else {
                logger.Info($"file {fileName} doesn't exist, cannot parse file into List<Movie>!");
               
                return outputList;
            }

        }
    }
}