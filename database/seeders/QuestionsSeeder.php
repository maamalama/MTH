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
            'name' => 'Вы первый раз в гараже?'
        ]);

        $answers[1][0] = Answers::create([
            'name' => 'Нет'
        ]);
        
        $answers[1][1] = Answers::create([
            'name' => 'Да'
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
            'name' => '1'
        ]);
        
        $answers[0][1] = Answers::create([
            'name' => '2'
        ]);
        
        $answers[0][2] = Answers::create([
            'name' => '3'
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
            'name' => '1'
        ]);
        
        $answers[2][1] = Answers::create([
            'name' => '2'
        ]);
        
        $answers[2][2] = Answers::create([
            'name' => '3'
        ]);

        foreach ($answers[0] as $value) {
            QuestionsAnswers::create([
                'question_id' => $question->id,
                'answer_id' => $value->id
            ]);
        }

        $question = Questions::create([
            'name' => 'Уютно ли вам у нас?'
        ]);

        $answers[3][0] = Answers::create([
            'name' => 'Да'
        ]);
        
        $answers[3][1] = Answers::create([
            'name' => 'Нет'
        ]);

        foreach ($answers[3] as $value) {
            QuestionsAnswers::create([
                'question_id' => $question->id,
                'answer_id' => $value->id
            ]);
        }

        $question = Questions::create([
            'name' => 'Как вы думаете Пикассо оценил бы эту комнату?'
        ]);

        $answers[4][0] = Answers::create([
            'name' => 'Да'
        ]);
        
        $answers[4][1] = Answers::create([
            'name' => 'Нет'
        ]);

        foreach ($answers[4] as $value) {
            QuestionsAnswers::create([
                'question_id' => $question->id,
                'answer_id' => $value->id
            ]);
        }

        $question = Questions::create([
            'name' => 'В каком веке была нарисована картина №2?'
        ]);

        $answers[5][0] = Answers::create([
            'name' => '15'
        ]);
        
        $answers[5][1] = Answers::create([
            'name' => '19'
        ]);

        foreach ($answers[5] as $value) {
            QuestionsAnswers::create([
                'question_id' => $question->id,
                'answer_id' => $value->id
            ]);
        }

    }
}
