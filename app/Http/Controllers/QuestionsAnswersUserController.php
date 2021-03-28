<?php

namespace App\Http\Controllers;

use App\Models\QuestionsAnswers;
use App\Models\QuestionsAnswersUser;
use Illuminate\Http\Request;

class QuestionsAnswersUserController extends Controller
{
    public function create(Request $request)
    {
        QuestionsAnswersUser::create([
            'user_id' => $request->user_id,
            'questions_answer_id' => $request->questions_answer_id,
        ]);

        return response()->json([$request->user_id, $request->questions_answer_id]);
    }
}
