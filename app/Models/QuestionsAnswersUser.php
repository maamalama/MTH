<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class QuestionsAnswersUser extends Model
{
    use HasFactory;

    protected $fillable = [
        'user_id',
        'questions_answer_id',
    ];

    public function user()
    {
        return $this->belongsTo(User::class);
    }

    public function questionsAnswer()
    {
        return $this->belongsTo(QuestionsAnswers::class);
    }
}
