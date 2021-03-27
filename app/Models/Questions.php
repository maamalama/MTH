<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class Questions extends Model
{
    use HasFactory;

    protected $fillable = [
        'name'
    ];

    public function questionsAnswer()
    {
        return $this->hasMany(QuestionsAnswers::class, 'question_id');
    }
}
