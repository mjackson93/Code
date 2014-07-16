/*
 * 
 */
package basic_tests;
import java.nio.file.*;
import java.nio.charset.*;
import java.io.*;
import java.util.*;
import java.util.regex.*;

import org.json.*;

public class Data_to_JSON {
	
	/* The number of parameters of data that each account has. */
	public static int NUM_OF_ACC_PARAMETERS = 7;	
	
	
	/* main
	 * ----
	 * Processes the given data file of account information and writes into a JSON file.
	 */
	public static void main(String[] args) {
		ArrayList<String> accounts_data = get_accounts_data(get_data_line("data.txt"));
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
			JSONArray account = new JSONArray();
			for (int j = 0; j < NUM_OF_ACC_PARAMETERS; j++)
			{
				JSONObject key_data = new JSONObject();
				try {
					key_data.put(acc_data_keys[j], tokens.nextToken());
				} catch (JSONException e) {
					e.printStackTrace();
				}
				account.put(key_data);
				
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
		System.out.println(account_database);
		
		try {
			FileWriter file = new FileWriter("data_to_json.json");
			file.write(account_database.toString());
			file.flush();
			file.close();
		} catch (IOException e) {
			e.printStackTrace();
		}
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
			//"[^\\||\\s]+\\|[^\\|]+\\|[^\\|]+\\|[^\\|]+\\|[^\\|]+\\|[^\\|]+\\|[^\\||\\s]+";
		Pattern pattern = Pattern.compile(patternStr);
		Matcher matcher = pattern.matcher(data);
		ArrayList<String> accounts = new ArrayList<String>();
		while (matcher.find())
		{
			accounts.add(matcher.group());
		}
		return accounts;
	}
	
	

}
