<?php

namespace App\Http\Controllers;

use App\Models\Questions;
use App\Models\User;
use Illuminate\Http\Request;

class QuestionsController extends Controller
{
    public function getQuestions(User $user)
    {
        $date_dif_m = date_diff(date_create($user->created_at), date_create())->i;

        // if ($date_dif_m > 15) {
        //     return response()->json(['question' => Questions::select('id','name')->with('questionsAnswer:id,question_id,answer_id','questionsAnswer.answer:id,name')->find(2)]);
        // }
        // if ($date_dif_m > 30) {
        //     return response()->json(['question' => Questions::select('id','name')->with('questionsAnswer:id,question_id,answer_id','questionsAnswer.answer:id,name')->find(3)]);
        // }
        // if ($date_dif_m > 45) {
        //     return response()->json(['question' => Questions::select('id','name')->with('questionsAnswer:id,question_id,answer_id','questionsAnswer.answer:id,name')->find(4)]);
        // }
        // if ($date_dif_m >= 59) {
        //     return response()->json(['question' => Questions::select('id','name')->with('questionsAnswer:id,question_id,answer_id','questionsAnswer.answer:id,name')->find(5)]);
        // }

        // return response()->json(['question' => Questions::select('id','name')->with('questionsAnswer:id,question_id,answer_id','questionsAnswer.answer:id,name')->find(6)]);
        
        return response()->json(['question' => Questions::select('id','name')->with('questionsAnswer:id,question_id,answer_id','questionsAnswer.answer:id,name')->get()]);
    }

    public function getFirstQuestions()
    {
        return response()->json(['question' => Questions::select('id','name')->with('questionsAnswer:id,question_id,answer_id','questionsAnswer.answer:id,name')->find(1)]);
    }
}
