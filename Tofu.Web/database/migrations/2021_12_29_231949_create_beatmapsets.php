<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateBeatmapsets extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('beatmapsets', function (Blueprint $table) {
			$table->bigInteger("beatmapset_id")->autoIncrement()->nullable(false)->index();
			$table->bigInteger("bancho_id")->index()->nullable(false);
			$table->bigInteger("creator_id")->default(0)->nullable(false);
			$table->string("creator")->index()->default("")->nullable(false);
			$table->string("artist")->index()->default("")->nullable(false);
			$table->string("title")->index()->default("")->nullable(false);
			$table->string("source")->index()->default("")->nullable(false);
			$table->string("tags")->index()->default("")->nullable(false);
			$table->string("ranked_by")->default("")->nullable(false);
			$table->tinyInteger("ranked_status")->default(0)->nullable(false);
			$table->timestamp("created_at")->default(DB::raw('CURRENT_TIMESTAMP'))->nullable(false);
			$table->timestamp("ranked_at")->default(DB::raw('CURRENT_TIMESTAMP'))->nullable(false);
			$table->float("rating")->nullable(false);
			$table->integer("count_rating")->nullable(false);
        });
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::dropIfExists('beatmapsets');
    }
}
