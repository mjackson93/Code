package basic_tests;
import java.io.BufferedReader;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.Reader;
import java.net.URL;
import java.nio.charset.Charset;
 
import java.util.ArrayList;
import java.util.Iterator;
import java.util.Map.Entry;

import com.google.gson.*;

import org.json.JSONException;
import org.json.JSONObject;
import org.json.JSONArray;

public class IMDb_api {

	/* The list of movies to search for by title. Currently, the top 5 of IMDb's top 250 movies. */
	public static String[] MOVIES_TO_FIND = {	
												"The Shawshank Redemption", 
												"The Godfather", 
												"The Godfather: Part II",
												"The Dark Knight", 
												"The Good, the Bad and the Ugly"
											}; 
	
	
	/* The name of the file that will be created to store the data */
	public static String JSON_FILENAME = "movie_data.json";
	
	
	
	/* main
	 * ----
	 * Collects data in json format for the movies specified in MOVIES_TO_FIND, re-packages them in
	 * a Json array sorted by the year the movie was released and writes them to a file, then opens
	 * the json file and prints the data out.
	 */
	public static void main(String[] args) throws IOException, JSONException {
		JSONObject[] movie_data = get_movie_data();
		JSONArray sorted_data = sort_by_release_year(movie_data);
		JSONObject packed_data = new JSONObject();
		packed_data.put("Movie Data", sorted_data);
		write_json_file(packed_data);
		read_json_file();
	}
	
	
	/* get_movie_data
	 * --------------
	 * Uses the API to search for and get the data via json format of the movies specified
	 * in MOVIES_TO_FIND. Returns the data via an array of Json Objects.
	 */
	private static JSONObject[] get_movie_data() throws IOException, JSONException
	{
		JSONObject[] data = new JSONObject[MOVIES_TO_FIND.length];
		for (int i = 0; i < MOVIES_TO_FIND.length; i++)
		{
			String name = MOVIES_TO_FIND[i];
			name = name.replace(" ", "%20");
			data[i] = readJsonFromUrl("http://www.omdbapi.com/?t=" + name);
		}
		return data;
	}
	
	
	/* sort_by_release_year
	 * --------------------
	 * Sorts the given JSONObject movie data by the release year of the movies and returns them in
	 * a json array (earliest to latest).
	 */
	private static JSONArray sort_by_release_year(JSONObject[] data) throws JSONException
	{
		for (int i = 0; i < data.length; i++)
		{
			int min_ind = i;
			for (int j = i+1; j < data.length; j++)
			{
				if (get_year(data[j]) < get_year(data[min_ind])) min_ind = j;
			}
			JSONObject temp = data[i];
			data[i] = data[min_ind];
			data[min_ind] = temp;
		}
		JSONArray array = new JSONArray();
		for (int i = 0; i < data.length; i++)
		{
			array.put(data[i]);
		}
		return array;
	}

	
	/* get_year
	 * --------
	 * Returns the year that the given movie was released.
	 */
	private static int get_year(JSONObject obj) throws JSONException
	{
		return Integer.parseInt(obj.getString("Released").substring(7));
	}
	
	
	/* write_json_file
	 * ---------------
	 * Takes the given json object and writes it to file under the name specified in JSON_FILENAME.
	 */
	private static void write_json_file(JSONObject obj) throws IOException
	{
		FileWriter file = new FileWriter(JSON_FILENAME);
		file.write(obj.toString());
		file.flush();
		file.close();
	}
	
	
	/* read_json_file
	 * --------------
	 * Reads the file under the name specified in JSON_FILENAME and prints it out.
	 */
	private static void read_json_file() throws JsonIOException, JsonSyntaxException, 
												FileNotFoundException
	{
		JsonParser parser = new JsonParser();
		JsonArray movie_data = new JsonArray();
		JsonObject obj = parser.parse(new FileReader(JSON_FILENAME)).getAsJsonObject();
		movie_data = obj.getAsJsonArray(obj.entrySet().iterator().next().getKey());
		for (int i = 0; i < movie_data.size(); i++)
		{
			JsonObject movie = movie_data.get(i).getAsJsonObject();
			System.out.print("Title: " + movie.get("Title").getAsString() + ";  ");
			System.out.print("Released: " + movie.get("Released").getAsString() + ";  ");
			System.out.print("IMDb Rating: " + movie.get("imdbRating").getAsString() + ";  ");
			System.out.print("Actors: " + movie.get("Actors").getAsString() + ";  ");
			
//			Iterator<Entry<String, JsonElement> > it = movie.entrySet().iterator();
//			while (it.hasNext())
//			{
//				Entry<String, JsonElement> next = it.next();
//				System.out.print(next.getKey() + " : " + next.getValue());
//				if (it.hasNext()) System.out.print(";  ");
//			}
			System.out.println("");
		}
	}
	
	
	
	
	/* Borrowed from https://gist.github.com/ninjabunny/bdc688443e6004b0d631 */
	private static String readAll(Reader rd) throws IOException {
		StringBuilder sb = new StringBuilder();
		int cp;
		while ((cp = rd.read()) != -1) {
			sb.append((char) cp);
		}
		return sb.toString();
	}
 
	/* Borrowed from https://gist.github.com/ninjabunny/bdc688443e6004b0d631 */
	public static JSONObject readJsonFromUrl(String url) throws IOException, JSONException {
		InputStream is = new URL(url).openStream();
		try {
			BufferedReader rd = new BufferedReader(new InputStreamReader(is, Charset.forName("UTF-8")));
			String jsonText = readAll(rd);
			JSONObject json = new JSONObject(jsonText);
			return json;
		} finally {
			is.close();
		}
	}
}
