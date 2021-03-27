<?php

namespace App\Http\Controllers;

use App\Models\Questions;
use App\Models\User;
use Illuminate\Http\Request;

class QuestionsController extends Controller
{
    public function getQuestions(User $user)
    {

        switch (date_diff(date_create($user->created_at), date_create())->m) {
            case 30:
                # code...
                break;

            case 60:
                # code...
                break;

            case 90:
                # code...
                break;

            default:
                # code...
                break;
        }
    }
}
