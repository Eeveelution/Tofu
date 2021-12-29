<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateScores extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('scores', function (Blueprint $table) {
			$table->bigInteger("score_id")->autoIncrement()->nullable(false)->index();
			$table->string("map_md5", 32)->default("")->nullable(false)->index();
			$table->bigInteger("user_id")->default(0)->nullable(false)->index();
			$table->bigInteger("score")->default(0)->nullable(false);
			$table->integer("max_combo")->default(0)->nullable(false);
			$table->tinyInteger("gamemode")->default(0)->nullable(false);
			$table->integer("hit300")->default(0)->nullable(false);
			$table->integer("hit100")->default(0)->nullable(false);
			$table->integer("hit50")->default(0)->nullable(false);
			$table->integer("hitMiss")->default(0)->nullable(false);
			$table->integer("hitGeki")->default(0)->nullable(false);
			$table->integer("hitKatu")->default(0)->nullable(false);
			$table->integer("mods")->default(0)->nullable(false);
			$table->string("grade", 4)->default("F")->nullable(false);
			$table->boolean("perfect")->default(false)->nullable(false);
			$table->boolean("passed")->default(false)->nullable(false);
			$table->boolean("ranked")->default(false)->nullable(false)->index();
			$table->string("submit_hash")->default("")->nullable(false)->index();
			$table->timestamp("submitted_at")->default(DB::raw('CURRENT_TIMESTAMP'))->nullable(false);
			$table->string("version", 16)->default("bYYYYMMDD.XX")->nullable(false);
        });
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::dropIfExists('scores');
    }
}
