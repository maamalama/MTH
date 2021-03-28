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
        if (($handle = fopen(public_path() . '/comment/comments.csv', 'r')) !== FALSE) {
            while (($data = fgetcsv($handle)) !== FALSE) {
                $full_date[] = $data[0];
                $full_date2[] = $data[1];
            }
            fclose($handle);
        }

        $count_date = count($full_date) - 1;

        // for ($i = 0; $i < 1000; $i++) {
        //     User::factory(1)->create([
        //         'comment' => $full_date[rand(0, $count_date)],
        //         'comment_positively' => (int) $full_date2[rand(0, $count_date)]
        //     ]);
        // }

        // for ($g = 0; $g < 1000; $g++) {
        //     switch ((rand(0, 4))) {
        //         case 0:
        //             $comment = 'Хороший';
        //             break;
        //         case 1:
        //             $comment = 'Хорошо';
        //             break;
        //         case 2:
        //             $comment = 'Понравилось';
        //             break;
        //         case 3:
        //             $comment = 'Интересно';
        //             break;
        //         case 4:
        //             $comment = 'Классно';
        //             break;

        //         default:
        //             # code...
        //             break;
        //     }
        //     User::factory(1)->create([
        //         'comment' => $comment,
        //         'comment_positively' => 0
        //     ]);
        // }

        for ($h = 0; $h < 5; $h++) {
            switch ((rand(0, 4))) {
                case 0:
                    $comment = 'Говно';
                    break;
                case 1:
                    $comment = 'Неинтересно';
                    break;
                case 2:
                    $comment = 'Блять';
                    break;
                case 3:
                    $comment = 'Плохо';
                    break;
                case 4:
                    $comment = 'Плохой';
                    break;

                default:
                    # code...
                    break;
            }
            User::factory(1)->create([
                'comment' => $comment,
                'comment_positively' => 1
            ]);
        }
    }
}
