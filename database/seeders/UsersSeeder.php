<?php

namespace Database\Seeders;

use App\Models\User;
use Illuminate\Database\Seeder;

class UsersSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        if (($handle = fopen(public_path().'/comment/comments.csv', 'r')) !== FALSE) {
            while (($data = fgetcsv($handle)) !== FALSE) {
                $full_date[] =$data[0];
                $full_date2[] =$data[1];
            }
            fclose($handle);
        }
        
        $count_date = count($full_date) - 1;

        for ($i=0; $i < 1000; $i++) { 
            User::factory(1)->create([
                'comment' => $full_date[rand(0, $count_date)],
                'comment_positively' => (int) $full_date2[rand(0, $count_date)]
            ]);
        }
    }
}
