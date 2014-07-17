/* Marcus Jackson
 * 
 */
package basic_tests;
import java.nio.file.*;
import java.nio.charset.*;
import java.io.*;
import java.util.*;
import java.util.Map.Entry;
import java.util.regex.*;
import com.google.gson.*;
import org.json.JSONObject;
import org.json.JSONArray;
import org.json.JSONException;



public class Data_to_JSON {
	
	/* The number of parameters of data that each account has. */
	public static int NUM_OF_ACC_PARAMETERS = 7;	
	
	
	/* main
	 * ----
	 * Processes the given data file of account information and writes into a JSON file.
	 */
	public static void main(String[] args) {
		ArrayList<String> accounts_data = get_accounts_data(get_data_line("data.txt"));
		JSONObject account_database = process_data(accounts_data);
//		System.out.println(account_database);
		try {
			FileWriter file = new FileWriter("data_to_json.json");
			file.write(account_database.toString());
			file.flush();
			file.close();
		} catch (IOException e) {
			e.printStackTrace();
		}
		read_json_file("data_to_json.json");
	}
	
	
	
	/* get_data_line
	 * ----------------
	 * Returns the given file data assuming that it is a single line.
	 */
	private static String get_data_line(String name)
	{
		Path file = FileSystems.getDefault().getPath(name);
		Charset charset = Charset.forName("US-ASCII");
		String line = "";
		try (BufferedReader reader = Files.newBufferedReader(file, charset)) {
		    line = reader.readLine();
		} catch (IOException x) {
		    System.err.format("IOException: %s%n", x);
		}
		return line;
	}
	
	
	
	/* get_accounts_data
	 * -----------------
	 * Uses regular expressions along with the Pattern class to split the data by account.
	 * Each element of the Array List will contain the 7 pieces of data for each account in
	 * a format that is ready to be tokenized by a simple tokenizer.
	 */
	private static ArrayList<String> get_accounts_data(String data)
	{
		String patternStr = "[^\\||\\s]+\\|([^\\|]+\\|){5}[^\\||\\s]+";
			//"[^\\||\\s]+\\|[^\\|]+\\|[^\\|]+\\|[^\\|]+\\|[^\\|]+\\|[^\\|]+\\|[^\\||\\s]+";  old
		Pattern pattern = Pattern.compile(patternStr);
		Matcher matcher = pattern.matcher(data);
		ArrayList<String> accounts = new ArrayList<String>();
		while (matcher.find())
		{
			accounts.add(matcher.group());
		}
		return accounts;
	}
	
	
	
	/* process_data
	 * ------------
	 * Processes the given Array List of strings that carry the data for the accounts and 
	 * encapsulates it in a json object. The hierarchy is a json object holding a json array of
	 * accounts as json objects with each type of info stored in the json object along with a
	 * string with that data for that account info type.
	 * 		i.e. Accounts Info -> (Array of Accounts: (firstName -> Bob; lastName -> Guy; etc) )
	 */
	private static JSONObject process_data(ArrayList<String> accounts_data)
	{
		StringTokenizer tokens = new StringTokenizer(accounts_data.remove(0), "|");
		String[] acc_data_keys = new String[NUM_OF_ACC_PARAMETERS];
		for (int i = 0; i < NUM_OF_ACC_PARAMETERS; i++)
		{
			acc_data_keys[i] = tokens.nextToken();
		}
		JSONArray accs_info = new JSONArray();
		for (int i = 0; i < accounts_data.size(); i++)
		{
			tokens = new StringTokenizer(accounts_data.get(i), "|");
			JSONObject account = new JSONObject();
			for (int j = 0; j < NUM_OF_ACC_PARAMETERS; j++)
			{
				try {
					account.put(acc_data_keys[j], tokens.nextToken());
				} catch (JSONException e) {
					e.printStackTrace();
				}
				
//				System.out.print(acc_data_keys[j] + " : " + tokens.nextToken());
//				if (j != NUM_OF_ACC_PARAMETERS - 1) System.out.print(", ");
			}
			accs_info.put(account);
//			System.out.println();
		}
		JSONObject account_database = new JSONObject();
		try {
			account_database.put("Accounts Information", accs_info);
		} catch (JSONException e) {
			e.printStackTrace();
		}
		return account_database;
	}
	
	
	
	/* read_json_file
	 * --------------
	 * Reads in the JSON file and prints out each element with its members.
	 */
	private static void read_json_file(String name)
	{
		JsonParser parser = new JsonParser();
		JsonArray accs_info = new JsonArray();
		try {
			JsonObject database = parser.parse(new FileReader(name)).getAsJsonObject();
			accs_info = database.getAsJsonArray("Accounts Information");
		} catch (JsonIOException | JsonSyntaxException | FileNotFoundException e) {
			e.printStackTrace();
		}
		for (int i = 0; i < accs_info.size(); i++)
		{
			JsonObject account = accs_info.get(i).getAsJsonObject();
			System.out.print("Elem " + (i+1) + ": ");
			Iterator<Entry<String, JsonElement> > it = account.entrySet().iterator();
			while (it.hasNext())
			{
				Entry<String, JsonElement> next = it.next();
				System.out.print(next.getKey() + " : " + next.getValue());
				if (it.hasNext()) System.out.print(";  ");
			}
			System.out.println("");
		}
	}
	

}
