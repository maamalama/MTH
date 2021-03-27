<?php

namespace App\Http\Controllers;

use App\Models\Questions;
use App\Models\QuestionsAnswers;
use App\Models\User;
use Illuminate\Http\Request;

class UserController extends Controller
{
    public function newUser(Request $request)
    {
        $user = User::create([
            'name' => $request->name,
            'lat' => $request->lat,
            'lon' => $request->lon,
            'sex' => $request->sex,
            'date_birth' => date('Y-m-d',strtotime($request->age)),
        ]);

        return response()->json(['user_id' => $user->id, 'question' => Questions::select('id','name')->with('questionsAnswer:id,question_id,answer_id','questionsAnswer.answer:id,name')->find(2)]);
    }

    public function getUsers()
    {
        return response()->json(['users' => User::all()]);
    }

    public function updateUser(Request $request, User $user)
    {
        $user->update($request->all());
    }

    public function test()
    {
        return response()->json(['date' => Questions::select('id','name')->with('questionsAnswer:id,question_id,answer_id','questionsAnswer.answer:id,name')->get()]);
    }
}
