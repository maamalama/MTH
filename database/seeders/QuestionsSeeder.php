<?php

namespace Database\Seeders;

use App\Models\Answers;
use App\Models\Questions;
use App\Models\QuestionsAnswers;
use Illuminate\Database\Seeder;

class QuestionsSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        $question = Questions::create([
            'name' => 'Вы первый раз в гараже'
        ]);

        $answers[1][0] = Answers::create([
            'Нет'
        ]);
        
        $answers[1][1] = Answers::create([
            'Да'
        ]);

        foreach ($answers[1] as $value) {
            QuestionsAnswers::create([
                'question_id' => $question->id,
                'answer_id' => $value->id
            ]);
        }

        $question = Questions::create([
            'name' => 'Какой экспонат вам понравился больше всего?'
        ]);

        $answers[0][0] = Answers::create([
            '1'
        ]);
        
        $answers[0][1] = Answers::create([
            '2'
        ]);
        
        $answers[0][2] = Answers::create([
            '3'
        ]);

        foreach ($answers[0] as $value) {
            QuestionsAnswers::create([
                'question_id' => $question->id,
                'answer_id' => $value->id
            ]);
        }

        $question = Questions::create([
            'name' => 'Какая инсталляции была более интересной?'
        ]);

        $answers[2][0] = Answers::create([
            '1'
        ]);
        
        $answers[2][1] = Answers::create([
            '2'
        ]);
        
        $answers[2][2] = Answers::create([
            '3'
        ]);

        foreach ($answers[0] as $value) {
            QuestionsAnswers::create([
                'question_id' => $question->id,
                'answer_id' => $value->id
            ]);
        }

    }
}
